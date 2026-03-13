export interface CommandLog {
  id: number;
  turbineId: number;
  issuedByUsername: string;
  issuedAt: string;
  commandType: string;
  targetRpm: number | null;
  status: string;
  notes: string | null;
}
