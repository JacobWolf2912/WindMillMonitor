namespace WindTurbineMonitor.Api.Mqtt.Payloads;

public record TelemetryPayload(
    string TurbineId,
    string TurbineName,
    string FarmId,
    string Timestamp,
    double WindSpeed,
    double WindDirection,
    double AmbientTemperature,
    double RotorSpeed,
    double PowerOutput,
    double NacelleDirection,
    double BladePitch,
    double GeneratorTemp,
    double GearboxTemp,
    double Vibration,
    string Status);
