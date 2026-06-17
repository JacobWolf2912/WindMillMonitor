import { useState, useEffect } from 'react';
import { get, post } from '../api/client';
import type { CommandLog } from '../types/command';

export function useCommands(turbineId: string) {
  const [commands, setCommands] = useState<CommandLog[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    get<CommandLog[]>(`/api/turbines/${turbineId}/commands`)
      .then(setCommands)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [turbineId]);

  const issueCommand = async (
    commandType: string,
    targetRpm?: number,
    issuedByUsername: string = 'operator'
  ): Promise<CommandLog> => {
    const result = await post<CommandLog>(`/api/turbines/${turbineId}/commands`, {
      commandType,
      targetRpm: targetRpm || null,
      issuedByUsername,
    });
    setCommands((prev) => [result, ...prev]);
    return result;
  };

  return { commands, loading, error, issueCommand };
}
