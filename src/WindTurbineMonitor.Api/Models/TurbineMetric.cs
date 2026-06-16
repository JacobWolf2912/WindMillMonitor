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

    // Temperature & Status metrics
    public double? NacelleTemperatureCelsius { get; set; }

    public double? GearboxTemperatureCelsius { get; set; }

    public TurbineStatus Status { get; set; }

    // Navigation property
    public Turbine Turbine { get; set; } = null!;
}
