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
    private const string FarmId = "5e5789ff-d103-45f1-97bf-e8086254c02f";

    [MqttRoute("farm/5e5789ff-d103-45f1-97bf-e8086254c02f/windmill/{turbineId}/telemetry")]
    public async Task OnTelemetry(string turbineId, TelemetryPayload payload)
    {
        try
        {
            var id = TurbineIdToNumber(turbineId);
            if (id <= 0)
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
                turbine = AutoRegisterTurbine(id, payload.TurbineName, payload.TurbineId);
                db.Turbines.Add(turbine);
                await db.SaveChangesAsync();
                logger.LogInformation("Auto-registered new turbine: {TurbineId} ({Name})", id, payload.TurbineName);
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
            logger.LogDebug("Persisted telemetry for turbine {TurbineId}", turbine.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing telemetry for turbine {TurbineId}", turbineId);
        }
    }

    private static int TurbineIdToNumber(string turbineId) => turbineId?.ToLower() switch
    {
        "turbine-alpha" => 1,
        "turbine-beta" => 2,
        "turbine-gamma" => 3,
        "turbine-delta" => 4,
        _ => 0
    };

    private static Turbine AutoRegisterTurbine(int turbineId, string turbineName, string mqttId)
    {
        return new Turbine
        {
            Id = turbineId,
            Name = turbineName,
            Location = "Offshore",
            MqttTopicPrefix = $"farm/{FarmId}/windmill/{mqttId}",
            InstalledAt = DateTime.UtcNow
        };
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
