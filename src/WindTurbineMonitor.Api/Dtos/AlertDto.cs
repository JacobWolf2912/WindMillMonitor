namespace WindTurbineMonitor.Api.Dtos;

public record AlertDto(
    int Id,
    int TurbineId,
    DateTime Timestamp,
    string Severity,
    string Title,
    string Description,
    bool IsAcknowledged,
    DateTime? AcknowledgedAt);
