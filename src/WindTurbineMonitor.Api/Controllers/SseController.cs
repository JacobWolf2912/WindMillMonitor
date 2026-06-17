using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using WindTurbineMonitor.Api.Data;
using WindTurbineMonitor.Api.Dtos;
using WindTurbineMonitor.Api.Services;

namespace WindTurbineMonitor.Api.Controllers;

[ApiController]
[Route("sse")]
public class SseController(
    AppDbContext db,
    MetricBroadcaster metricBroadcaster,
    AlertBroadcaster alertBroadcaster) : ControllerBase
{
    [HttpGet("turbines/{turbineId}")]
    public async Task StreamTurbineMetrics(string turbineId)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        try
        {
            // Send initial metric
            var initialMetric = await GetLatestMetricForTurbine(turbineId);
            if (initialMetric != null)
            {
                await SendSseEventAsync(initialMetric);
            }

            // Stream new metrics as they arrive
            var reader = metricBroadcaster.Subscribe(turbineId);

            await foreach (var metric in reader.ReadAllAsync(HttpContext.RequestAborted))
            {
                await SendSseEventAsync(metric);
            }
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

        try
        {
            // Send initial alert
            var initialAlert = await GetLatestAlert();
            if (initialAlert != null)
            {
                await SendSseEventAsync(initialAlert);
            }

            // Stream new alerts as they arrive
            var reader = alertBroadcaster.Subscribe();
            await foreach (var alert in reader.ReadAllAsync(HttpContext.RequestAborted))
            {
                await SendSseEventAsync(alert);
            }
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

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(data, options);
        await Response.WriteAsync($"data: {json}\n\n");
        await Response.Body.FlushAsync();
    }

    private async Task<MetricDto?> GetLatestMetricForTurbine(string turbineId)
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
            metric.AmbientTemperatureCelsius, metric.NacelleDirectionDeg, metric.BladePitchDeg,
            metric.GeneratorTemperatureCelsius, metric.GearboxTemperatureCelsius, metric.VibrationMs2,
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
