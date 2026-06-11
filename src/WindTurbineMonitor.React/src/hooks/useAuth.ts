import { useState, useCallback } from 'react';
import { API_BASE } from '../api/client';

export interface LoginResponse {
  token: string;
  username: string;
  expiresIn: number;
}

export function useAuth() {
  const [isLoggedIn, setIsLoggedIn] = useState(() => {
    return !!localStorage.getItem('jwt_token');
  });
  const [username, setUsername] = useState(() => {
    return localStorage.getItem('username') || '';
  });
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const login = useCallback(async (username: string, password: string) => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await fetch(`${API_BASE}/api/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password }),
      });

      if (!response.ok) {
        throw new Error(`Login failed: ${response.status}`);
      }

      const data: LoginResponse = await response.json();

      localStorage.setItem('jwt_token', data.token);
      localStorage.setItem('username', data.username);
      setIsLoggedIn(true);
      setUsername(data.username);

      // Reload page to ensure App component sees updated auth state
      window.location.reload();

      return data;
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Login failed';
      setError(message);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('username');
    setIsLoggedIn(false);
    setUsername('');
    setError(null);
  }, []);

  return {
    isLoggedIn,
    username,
    error,
    isLoading,
    login,
    logout,
  };
}
