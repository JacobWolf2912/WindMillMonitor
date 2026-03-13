using Microsoft.EntityFrameworkCore;
using Mqtt.Controllers;
using WindTurbineMonitor.Api.Data;
using WindTurbineMonitor.Api.Models;
using WindTurbineMonitor.Api.Models.Enums;
using WindTurbineMonitor.Api.Mqtt.Payloads;
using WindTurbineMonitor.Api.Services;

namespace WindTurbineMonitor.Api.Mqtt.Controllers;

public class TurbineMetricsController(
    IServiceScopeFactory scopeFactory,
    AlertEvaluationService alertService,
    ILogger<TurbineMetricsController> logger) : MqttController
{
    [MqttRoute("fsiot/windturbines/{turbineId}/metrics")]
    public async Task OnMetrics(string turbineId, MetricPayload payload)
    {
        try
        {
            if (!int.TryParse(turbineId, out var id))
            {
                logger.LogWarning("Received metric with invalid turbineId: {TurbineId}", turbineId);
                return;
            }

            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Resolve or auto-register the turbine
            var turbine = await db.Turbines.FindAsync(id);
            if (turbine == null)
            {
                turbine = AutoRegisterTurbine(id);
                db.Turbines.Add(turbine);
                await db.SaveChangesAsync();
                logger.LogInformation("Auto-registered new turbine: {TurbineId}", id);
            }

            // Persist metric
            var metric = new TurbineMetric
            {
                TurbineId = turbine.Id,
                Timestamp = DateTime.UtcNow,
                RotorRpm = payload.RotorRpm,
                PowerOutputKw = payload.PowerOutputKw,
                WindSpeedMs = payload.WindSpeedMs,
                WindDirectionDeg = payload.WindDirectionDeg,
                NacelleTemperatureCelsius = payload.NacelleTemperatureCelsius,
                GearboxTemperatureCelsius = payload.GearboxTemperatureCelsius,
                Status = ParseStatus(payload.Status) ?? TurbineStatus.Online
            };
            db.TurbineMetrics.Add(metric);

            // Evaluate thresholds → produce alerts
            var alerts = alertService.Evaluate(turbine.Id, payload).ToList();
            if (alerts.Count > 0)
            {
                db.Alerts.AddRange(alerts);
                logger.LogWarning("Generated {AlertCount} alerts for turbine {TurbineId}",
                    alerts.Count, turbine.Id);
            }

            await db.SaveChangesAsync();
            logger.LogDebug("Persisted metric for turbine {TurbineId}", turbine.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing metric for turbine {TurbineId}", turbineId);
        }
    }

    private static Turbine AutoRegisterTurbine(int turbineId)
    {
        return new Turbine
        {
            Id = turbineId,
            Name = $"Turbine-{turbineId:D2}",
            Location = "Offshore",
            MqttTopicPrefix = $"fsiot/windturbines/{turbineId}",
            InstalledAt = DateTime.UtcNow
        };
    }

    private static TurbineStatus? ParseStatus(string? status)
    {
        return status switch
        {
            "Online" => TurbineStatus.Online,
            "Offline" => TurbineStatus.Offline,
            "Fault" => TurbineStatus.Fault,
            "Maintenance" => TurbineStatus.Maintenance,
            _ => null
        };
    }
}
