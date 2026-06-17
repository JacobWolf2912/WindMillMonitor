using WindTurbineMonitor.Api.Models;
using WindTurbineMonitor.Api.Models.Enums;
using WindTurbineMonitor.Api.Mqtt.Payloads;

namespace WindTurbineMonitor.Api.Services;

public class AlertEvaluationService
{
    /// <summary>
    /// Evaluates a metric payload against threshold rules and returns alert records for violations.
    /// </summary>
    public IEnumerable<Alert> Evaluate(string turbineId, TelemetryPayload payload)
    {
        var alerts = new List<Alert>();

        // RPM thresholds
        if (payload.RotorSpeed > 0)
        {
            if (payload.RotorSpeed > 35)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Critical,
                    "Critical RPM",
                    $"Rotor RPM exceeds critical threshold: {payload.RotorSpeed} RPM"));
            }
            else if (payload.RotorSpeed > 30)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Warning,
                    "Warning RPM",
                    $"Rotor RPM exceeds warning threshold: {payload.RotorSpeed} RPM"));
            }
        }

        // Generator temperature thresholds
        if (payload.GeneratorTemp > 0)
        {
            if (payload.GeneratorTemp > 90)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Critical,
                    "Critical Generator Temperature",
                    $"Generator temperature exceeds critical threshold: {payload.GeneratorTemp} °C"));
            }
            else if (payload.GeneratorTemp > 80)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Warning,
                    "Warning Generator Temperature",
                    $"Generator temperature exceeds warning threshold: {payload.GeneratorTemp} °C"));
            }
        }

        // Gearbox temperature thresholds
        if (payload.GearboxTemp > 0)
        {
            if (payload.GearboxTemp > 120)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Critical,
                    "Critical Gearbox Temperature",
                    $"Gearbox temperature exceeds critical threshold: {payload.GearboxTemp} °C"));
            }
            else if (payload.GearboxTemp > 100)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Warning,
                    "Warning Gearbox Temperature",
                    $"Gearbox temperature exceeds warning threshold: {payload.GearboxTemp} °C"));
            }
        }

        // Wind speed thresholds
        if (payload.WindSpeed > 0)
        {
            if (payload.WindSpeed > 30)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Critical,
                    "Extreme Wind Speed",
                    $"Wind speed exceeds critical threshold: {payload.WindSpeed} m/s"));
            }
            else if (payload.WindSpeed > 25)
            {
                alerts.Add(CreateAlert(turbineId, AlertSeverity.Warning,
                    "High Wind Speed",
                    $"Wind speed exceeds warning threshold: {payload.WindSpeed} m/s"));
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
