using WindTurbineMonitor.Api.Models.Enums;

namespace WindTurbineMonitor.Api.Models;

public class CommandLog
{
    public long Id { get; set; }

    public int TurbineId { get; set; }

    public required string IssuedByUsername { get; set; }

    public DateTime IssuedAt { get; set; }

    public CommandType CommandType { get; set; }

    public string? ParametersJson { get; set; }

    public CommandStatus Status { get; set; }

    public string? Notes { get; set; }

    // Navigation property
    public Turbine Turbine { get; set; } = null!;
}
