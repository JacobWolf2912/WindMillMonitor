using WindTurbineMonitor.Api.Models.Enums;

namespace WindTurbineMonitor.Api.Models;

public class TurbineMetric
{
    public long Id { get; set; }

    public required string TurbineId { get; set; }

    public DateTime Timestamp { get; set; }

    // RPM & Power metrics
    public double? RotorRpm { get; set; }

    public double? PowerOutputKw { get; set; }

    // Wind & Environment metrics
    public double? WindSpeedMs { get; set; }

    public double? WindDirectionDeg { get; set; }

    public double? AmbientTemperatureCelsius { get; set; }

    public double? NacelleDirectionDeg { get; set; }

    public double? BladePitchDeg { get; set; }

    // Temperature & Status metrics
    public double? GeneratorTemperatureCelsius { get; set; }

    public double? GearboxTemperatureCelsius { get; set; }

    public double? VibrationMs2 { get; set; }

    public TurbineStatus Status { get; set; }

    // Navigation property
    public Turbine Turbine { get; set; } = null!;
}
