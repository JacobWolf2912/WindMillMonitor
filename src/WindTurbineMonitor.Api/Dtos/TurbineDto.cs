namespace WindTurbineMonitor.Api.Dtos;

public record TurbineDto(
    int Id,
    string Name,
    string Location,
    DateTime InstalledAt);
