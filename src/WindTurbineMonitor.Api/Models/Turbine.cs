namespace WindTurbineMonitor.Api.Models;

public class Turbine
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Location { get; set; }

    public required string MqttTopicPrefix { get; set; }

    public DateTime InstalledAt { get; set; }

    // Navigation properties
    public ICollection<TurbineMetric> Metrics { get; set; } = [];

    public ICollection<Alert> Alerts { get; set; } = [];

    public ICollection<CommandLog> CommandLogs { get; set; } = [];
}
