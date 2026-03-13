using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindTurbineMonitor.Api.Data;
using WindTurbineMonitor.Api.Dtos;
using System.Text.Json;

namespace WindTurbineMonitor.Api.Controllers;

[ApiController]
[Route("sse")]
public class SseController(AppDbContext db) : ControllerBase
{
    [HttpGet("turbines/{turbineId}")]
    public async Task StreamTurbineMetrics(int turbineId)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        // Get initial metric and send it
        var initialMetric = await GetLatestMetricForTurbine(turbineId);
        await SendSseEventAsync(initialMetric);

        // Keep connection open
        try
        {
            await Task.Delay(Timeout.Infinite, HttpContext.RequestAborted);
        }
        catch (OperationCanceledException)
        {
            // Client disconnected
        }
    }

    [HttpGet("alerts")]
    public async Task StreamAlerts()
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        // Get initial alert and send it
        var initialAlert = await GetLatestAlert();
        await SendSseEventAsync(initialAlert);

        // Keep connection open
        try
        {
            await Task.Delay(Timeout.Infinite, HttpContext.RequestAborted);
        }
        catch (OperationCanceledException)
        {
            // Client disconnected
        }
    }

    private async Task SendSseEventAsync<T>(T data)
    {
        if (data == null)
            return;

        var json = JsonSerializer.Serialize(data);
        await Response.WriteAsync($"data: {json}\n\n");
        await Response.Body.FlushAsync();
    }

    private async Task<MetricDto?> GetLatestMetricForTurbine(int turbineId)
    {
        var metric = await db.TurbineMetrics
            .Where(m => m.TurbineId == turbineId)
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefaultAsync();

        if (metric == null)
            return null;

        return new MetricDto(
            metric.Id, metric.TurbineId, metric.Timestamp,
            metric.RotorRpm, metric.PowerOutputKw,
            metric.WindSpeedMs, metric.WindDirectionDeg,
            metric.NacelleTemperatureCelsius, metric.GearboxTemperatureCelsius,
            metric.Status.ToString());
    }

    private async Task<AlertDto?> GetLatestAlert()
    {
        var alert = await db.Alerts
            .OrderByDescending(a => a.Timestamp)
            .FirstOrDefaultAsync();

        if (alert == null)
            return null;

        return new AlertDto(
            alert.Id, alert.TurbineId, alert.Timestamp,
            alert.Severity.ToString(), alert.Title, alert.Description,
            alert.IsAcknowledged, alert.AcknowledgedAt);
    }
}
