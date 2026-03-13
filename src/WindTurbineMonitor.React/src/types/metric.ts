export interface Metric {
  id: number;
  turbineId: number;
  timestamp: string;
  rotorRpm: number | null;
  powerOutputKw: number | null;
  windSpeedMs: number | null;
  windDirectionDeg: number | null;
  nacelleTemperatureCelsius: number | null;
  gearboxTemperatureCelsius: number | null;
  status: string;
}
