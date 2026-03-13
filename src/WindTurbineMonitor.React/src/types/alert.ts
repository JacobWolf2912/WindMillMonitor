export interface Alert {
  id: number;
  turbineId: number;
  timestamp: string;
  severity: string;
  title: string;
  description: string;
  isAcknowledged: boolean;
  acknowledgedAt: string | null;
}
