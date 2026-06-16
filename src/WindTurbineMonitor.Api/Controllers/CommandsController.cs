using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mqtt.Controllers;
using WindTurbineMonitor.Api.Data;
using WindTurbineMonitor.Api.Dtos;
using WindTurbineMonitor.Api.Models;
using WindTurbineMonitor.Api.Models.Enums;

namespace WindTurbineMonitor.Api.Controllers;

[ApiController]
[Route("api/turbines/{turbineId}/commands")]
[Authorize]
public class CommandsController(AppDbContext db, IMqttClientService mqtt, ILogger<CommandsController> logger)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CommandLogDto>> IssueCommand(
        string turbineId,
        [FromBody] IssueCommandRequest request)
    {
        // Validate turbine exists
        var turbine = await db.Turbines.FindAsync(turbineId);
        if (turbine == null)
            return NotFound($"Turbine {turbineId} not found");

        // Validate CommandType
        if (!Enum.TryParse<CommandType>(request.CommandType, ignoreCase: true, out var commandType))
            return BadRequest($"Invalid CommandType: {request.CommandType}");

        // For SetTargetRpm, validate TargetRpm
        if (commandType == CommandType.SetTargetRpm)
        {
            if (!request.TargetRpm.HasValue)
                return BadRequest("TargetRpm is required for SetTargetRpm command");
            if (request.TargetRpm < 0 || request.TargetRpm > 50)
                return BadRequest("TargetRpm must be between 0 and 50");
        }

        // Create CommandLog with Pending status
        var commandLog = new CommandLog
        {
            TurbineId = turbineId,
            IssuedByUsername = request.IssuedByUsername,
            CommandType = commandType,
            ParametersJson = request.TargetRpm.HasValue ? JsonSerializer.Serialize(new { TargetRpm = request.TargetRpm }) : null,
            Status = CommandStatus.Pending,
            IssuedAt = DateTime.UtcNow
        };

        db.CommandLogs.Add(commandLog);
        await db.SaveChangesAsync();

        // Publish to MQTT
        var payload = new
        {
            commandType = request.CommandType,
            targetRpm = request.TargetRpm,
            issuedAt = DateTime.UtcNow.ToString("O"),
            issuedByUsername = request.IssuedByUsername
        };

        var topic = $"fsiot/windturbines/{turbineId}/commands";
        await mqtt.PublishAsync(topic, JsonSerializer.Serialize(payload));
        logger.LogInformation("Published command {Type} to {Topic}", request.CommandType, topic);

        // Update status to Executed
        commandLog.Status = CommandStatus.Executed;
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCommands), new { turbineId }, MapToDto(commandLog));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommandLogDto>>> GetCommands(
        string turbineId,
        [FromQuery] int limit = 50)
    {
        var commands = await db.CommandLogs
            .Where(c => c.TurbineId == turbineId)
            .OrderByDescending(c => c.IssuedAt)
            .Take(limit)
            .Select(c => MapToDto(c))
            .ToListAsync();

        return Ok(commands);
    }

    private static CommandLogDto MapToDto(CommandLog c) =>
        new(c.Id, c.TurbineId, c.IssuedByUsername, c.IssuedAt,
            c.CommandType.ToString(), ExtractTargetRpm(c.ParametersJson), c.Status.ToString(), c.Notes);

    private static double? ExtractTargetRpm(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return null;
        try
        {
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("TargetRpm", out var rpm))
                return rpm.GetDouble();
        }
        catch { }
        return null;
    }
}
