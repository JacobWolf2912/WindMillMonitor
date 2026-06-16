namespace WindTurbineMonitor.Api.Dtos;

public record TurbineDto(
    string Id,
    string Name,
    string Location,
    DateTime InstalledAt);
