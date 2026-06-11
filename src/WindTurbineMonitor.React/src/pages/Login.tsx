import { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import './Login.css';

interface LoginPageProps {
  onLoginSuccess: () => void;
}

export function LoginPage({ onLoginSuccess }: LoginPageProps) {
  const { login, isLoading, error } = useAuth();
  const [inputUsername, setInputUsername] = useState('');
  const [inputPassword, setInputPassword] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await login(inputUsername, inputPassword);
      onLoginSuccess();
    } catch {
      // Error is handled by useAuth hook
    }
  };

  return (
    <div className="login-container">
      <div className="login-card">
        <h1 className="login-title">💨 Wind Turbine Monitor</h1>
        <p className="login-subtitle">Windmill Inspection Centre</p>

        <form onSubmit={handleSubmit} className="login-form">
          <div className="form-group">
            <label htmlFor="username">Username</label>
            <input
              id="username"
              type="text"
              value={inputUsername}
              onChange={(e) => setInputUsername(e.target.value)}
              placeholder="Enter your username"
              disabled={isLoading}
              className="form-input"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Password</label>
            <input
              id="password"
              type="password"
              value={inputPassword}
              onChange={(e) => setInputPassword(e.target.value)}
              placeholder="Enter your password"
              disabled={isLoading}
              className="form-input"
              required
            />
          </div>

          {error && <div className="error-message">{error}</div>}

          <button
            type="submit"
            disabled={isLoading || !inputUsername.trim()}
            className="submit-btn"
          >
            {isLoading ? 'Signing in...' : 'Sign In'}
          </button>
        </form>

        <p className="login-footer">Demo: Enter any username to get started</p>
      </div>
    </div>
  );
}
