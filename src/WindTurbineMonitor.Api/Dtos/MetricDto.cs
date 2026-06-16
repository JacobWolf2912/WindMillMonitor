namespace WindTurbineMonitor.Api.Dtos;

public record MetricDto(
    long Id,
    string TurbineId,
    DateTime Timestamp,
    double? RotorRpm,
    double? PowerOutputKw,
    double? WindSpeedMs,
    double? WindDirectionDeg,
    double? NacelleTemperatureCelsius,
    double? GearboxTemperatureCelsius,
    string Status);
