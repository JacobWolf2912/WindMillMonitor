namespace WindTurbineMonitor.Api.Dtos;

public record MetricDto(
    long Id,
    string TurbineId,
    DateTime Timestamp,
    double? RotorRpm,
    double? PowerOutputKw,
    double? WindSpeedMs,
    double? WindDirectionDeg,
    double? AmbientTemperatureCelsius,
    double? NacelleDirectionDeg,
    double? BladePitchDeg,
    double? GeneratorTemperatureCelsius,
    double? GearboxTemperatureCelsius,
    double? VibrationMs2,
    string Status);
