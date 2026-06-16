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
    ILogger<TurbineMetricsController> logger) : MqttController
{
    private const string FarmId = "5e5789ff-d103-45f1-97bf-e8086254c02f";

    [MqttRoute("farm/5e5789ff-d103-45f1-97bf-e8086254c02f/windmill/{turbineId}/telemetry")]
    public async Task OnTelemetry(string turbineId, TelemetryPayload payload)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Look up turbine by ID
            var turbine = await db.Turbines.FindAsync(turbineId);

            if (turbine == null)
            {
                logger.LogWarning("Received telemetry for unknown turbine: {TurbineId}", turbineId);
                return;
            }

            // Persist metric (map sea-fullstack fields to our schema)
            var metric = new TurbineMetric
            {
                TurbineId = turbine.Id,
                Timestamp = DateTime.Parse(payload.Timestamp),
                RotorRpm = payload.RotorSpeed,
                PowerOutputKw = payload.PowerOutput,
                WindSpeedMs = payload.WindSpeed,
                WindDirectionDeg = payload.WindDirection,
                NacelleTemperatureCelsius = payload.AmbientTemperature,
                GearboxTemperatureCelsius = payload.GearboxTemp,
                Status = ParseStatus(payload.Status) ?? TurbineStatus.Online
            };
            db.TurbineMetrics.Add(metric);
            await db.SaveChangesAsync();
            logger.LogInformation("Persisted telemetry from {TurbineName}", turbine.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing telemetry for turbine {TurbineId}", turbineId);
        }
    }


    private static TurbineStatus? ParseStatus(string? status)
    {
        return status?.ToLower() switch
        {
            "running" => TurbineStatus.Online,
            "stopped" => TurbineStatus.Offline,
            "online" => TurbineStatus.Online,
            "offline" => TurbineStatus.Offline,
            "fault" => TurbineStatus.Fault,
            "maintenance" => TurbineStatus.Maintenance,
            _ => null
        };
    }
}
