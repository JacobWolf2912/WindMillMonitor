using WindTurbineMonitor.Api.Models.Enums;

namespace WindTurbineMonitor.Api.Models;

public class Alert
{
    public int Id { get; set; }

    public required string TurbineId { get; set; }

    public DateTime Timestamp { get; set; }

    public AlertSeverity Severity { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public bool IsAcknowledged { get; set; } = false;

    public DateTime? AcknowledgedAt { get; set; }

    // Navigation property
    public Turbine Turbine { get; set; } = null!;
}
