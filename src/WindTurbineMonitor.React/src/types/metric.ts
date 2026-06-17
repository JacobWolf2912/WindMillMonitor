export interface Metric {
  id: number;
  turbineId: string;
  timestamp: string;
  rotorRpm: number | null;
  powerOutputKw: number | null;
  windSpeedMs: number | null;
  windDirectionDeg: number | null;
  ambientTemperatureCelsius: number | null;
  nacelleDirectionDeg: number | null;
  bladePitchDeg: number | null;
  generatorTemperatureCelsius: number | null;
  gearboxTemperatureCelsius: number | null;
  vibrationMs2: number | null;
  status: string;
}
