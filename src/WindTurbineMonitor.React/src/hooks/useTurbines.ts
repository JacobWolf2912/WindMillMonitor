import { useState, useEffect } from 'react';
import type { Turbine } from '../types/turbine';
import { get } from '../api/client';

export function useTurbines() {
  const [turbines, setTurbines] = useState<Turbine[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    get<Turbine[]>('/api/turbines')
      .then(setTurbines)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  return { turbines, loading, error };
}
