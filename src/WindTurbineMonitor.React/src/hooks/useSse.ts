import { useEffect } from 'react';
import { API_BASE } from '../api/client';

export function useSse<T>(endpoint: string, onMessage: (data: T) => void) {
  useEffect(() => {
    const eventSource = new EventSource(`${API_BASE}${endpoint}`);

    eventSource.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);
        onMessage(data);
      } catch (err) {
        console.error('Failed to parse SSE message:', err);
      }
    };

    eventSource.onerror = () => {
      console.error('SSE error, closing connection');
      eventSource.close();
    };

    return () => eventSource.close();
  }, [endpoint, onMessage]);
}
