namespace WindTurbineMonitor.Api.Dtos;

public record AlertDto(
    int Id,
    string TurbineId,
    DateTime Timestamp,
    string Severity,
    string Title,
    string Description,
    bool IsAcknowledged,
    DateTime? AcknowledgedAt);
