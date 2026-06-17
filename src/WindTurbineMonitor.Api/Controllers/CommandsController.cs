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
public class CommandsController(
    AppDbContext db,
    IMqttClientService mqtt,
    ILogger<CommandsController> logger,
    IConfiguration configuration)
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

        // Publish to MQTT with simulator-compatible format
        var farmId = configuration["FarmId"] ?? "5e5789ff-d103-45f1-97bf-e8086254c02f";
        var topic = $"farm/{farmId}/windmill/{turbineId}/command";

        var command = BuildMqttCommand(commandType, request.TargetRpm, request.Reason);
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var payload = JsonSerializer.Serialize(command, jsonOptions);

        await mqtt.PublishAsync(topic, payload);
        logger.LogInformation("Published command {Type} to {Topic} with payload {Payload}", request.CommandType, topic, payload);

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

    private static object BuildMqttCommand(CommandType commandType, double? targetRpm, string? reason)
    {
        return commandType switch
        {
            CommandType.Start => new { action = "start" },
            CommandType.Stop => string.IsNullOrEmpty(reason)
                ? new { action = "stop" }
                : new { action = "stop", reason },
            CommandType.SetTargetRpm => new { action = "setInterval", value = (int?)(targetRpm ?? 30) },
            CommandType.EmergencyStop => new { action = "stop", reason = "Emergency stop triggered" },
            _ => new { action = "start" }
        };
    }
}

public record IssueCommandRequest(
    string CommandType,
    double? TargetRpm = null,
    string? IssuedByUsername = null,
    string? Reason = null);
