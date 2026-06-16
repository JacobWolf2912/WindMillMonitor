namespace WindTurbineMonitor.Api.Dtos;

public record IssueCommandRequest(string CommandType, double? TargetRpm, string IssuedByUsername);
public record CommandLogDto(
    long Id,
    string TurbineId,
    string IssuedByUsername,
    DateTime IssuedAt,
    string CommandType,
    double? TargetRpm,
    string Status,
    string? Notes);
