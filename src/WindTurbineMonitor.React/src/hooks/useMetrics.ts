import { useState, useEffect } from 'react';
import type { Metric } from '../types/metric';
import { get } from '../api/client';

export function useMetrics(turbineId: number, limit = 100) {
  const [metrics, setMetrics] = useState<Metric[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    get<Metric[]>(`/api/turbines/${turbineId}/metrics?limit=${limit}`)
      .then(setMetrics)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [turbineId, limit]);

  return { metrics, loading, error };
}

export function useLatestMetric(turbineId: number) {
  const [metric, setMetric] = useState<Metric | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    get<Metric>(`/api/turbines/${turbineId}/metrics/latest`)
      .then(setMetric)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [turbineId]);

  return { metric, loading, error };
}
