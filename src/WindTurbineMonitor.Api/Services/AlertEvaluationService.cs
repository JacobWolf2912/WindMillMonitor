using WindTurbineMonitor.Api.Models;
using WindTurbineMonitor.Api.Models.Enums;
using WindTurbineMonitor.Api.Mqtt.Payloads;

namespace WindTurbineMonitor.Api.Services;

public class AlertEvaluationService
{
    /// <summary>
    /// Evaluates a metric payload against threshold rules and returns alert records for violations.
    /// </summary>
    public IEnumerable<Alert> Evaluate(string turbineId, MetricPayload payload)
    {
        var alerts = new List<Alert>();

        // RPM thresholds
        if (payload.RotorRpm.HasValue)
        {
            if (payload.RotorRpm > 35)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Critical,
                    "Critical RPM",
                    $"Rotor RPM exceeds critical threshold: {payload.RotorRpm} RPM"));
            }
            else if (payload.RotorRpm > 30)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Warning,
                    "Warning RPM",
                    $"Rotor RPM exceeds warning threshold: {payload.RotorRpm} RPM"));
            }
        }

        // Nacelle temperature thresholds
        if (payload.NacelleTemperatureCelsius.HasValue)
        {
            if (payload.NacelleTemperatureCelsius > 90)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Critical,
                    "Critical Nacelle Temperature",
                    $"Nacelle temperature exceeds critical threshold: {payload.NacelleTemperatureCelsius} °C"));
            }
            else if (payload.NacelleTemperatureCelsius > 80)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Warning,
                    "Warning Nacelle Temperature",
                    $"Nacelle temperature exceeds warning threshold: {payload.NacelleTemperatureCelsius} °C"));
            }
        }

        // Gearbox temperature thresholds
        if (payload.GearboxTemperatureCelsius.HasValue)
        {
            if (payload.GearboxTemperatureCelsius > 120)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Critical,
                    "Critical Gearbox Temperature",
                    $"Gearbox temperature exceeds critical threshold: {payload.GearboxTemperatureCelsius} °C"));
            }
            else if (payload.GearboxTemperatureCelsius > 100)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Warning,
                    "Warning Gearbox Temperature",
                    $"Gearbox temperature exceeds warning threshold: {payload.GearboxTemperatureCelsius} °C"));
            }
        }

        // Wind speed thresholds
        if (payload.WindSpeedMs.HasValue)
        {
            if (payload.WindSpeedMs > 30)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Critical,
                    "Extreme Wind Speed",
                    $"Wind speed exceeds critical threshold: {payload.WindSpeedMs} m/s"));
            }
            else if (payload.WindSpeedMs > 25)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Warning,
                    "High Wind Speed",
                    $"Wind speed exceeds warning threshold: {payload.WindSpeedMs} m/s"));
            }
        }

        // Status-based alerts
        if (!string.IsNullOrEmpty(payload.Status) && payload.Status == "Fault")
        {
            alerts.Add(CreateAlert(turbineId, AlertSeverity.Critical,
                "Turbine Fault",
                "Turbine is reporting a fault status"));
        }

        return alerts;
    }

    private static Alert CreateAlert(string turbineId, AlertSeverity severity, string title, string description)
    {
        return new Alert
        {
            TurbineId = turbineId,
            Severity = severity,
            Title = title,
            Description = description,
            IsAcknowledged = false,
            Timestamp = DateTime.UtcNow
        };
    }
}
