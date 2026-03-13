import { useState, useEffect } from 'react';
import type { Alert } from '../types/alert';
import { get, patch } from '../api/client';

export function useAlerts(turbineId?: number) {
  const [alerts, setAlerts] = useState<Alert[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchAlerts = async () => {
      const path = turbineId
        ? `/api/alerts?turbineId=${turbineId}`
        : '/api/alerts';
      try {
        const data = await get<Alert[]>(path);
        setAlerts(data);
        setError(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to fetch alerts');
      } finally {
        setLoading(false);
      }
    };

    fetchAlerts();

    // Poll for new alerts every 5 seconds
    const interval = setInterval(fetchAlerts, 5000);
    return () => clearInterval(interval);
  }, [turbineId]);

  const acknowledge = async (alertId: number) => {
    try {
      await patch(`/api/alerts/${alertId}/acknowledge`);
      setAlerts((prev) =>
        prev.map((a) =>
          a.id === alertId
            ? { ...a, isAcknowledged: true, acknowledgedAt: new Date().toISOString() }
            : a
        )
      );
    } catch (err) {
      console.error('Failed to acknowledge alert:', err);
    }
  };

  return { alerts, loading, error, acknowledge };
}
