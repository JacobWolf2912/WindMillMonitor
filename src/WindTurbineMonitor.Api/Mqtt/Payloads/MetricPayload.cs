namespace WindTurbineMonitor.Api.Mqtt.Payloads;

public record MetricPayload(
    double? RotorRpm,
    double? PowerOutputKw,
    double? WindSpeedMs,
    double? WindDirectionDeg,
    double? NacelleTemperatureCelsius,
    double? GearboxTemperatureCelsius,
    string? Status);
