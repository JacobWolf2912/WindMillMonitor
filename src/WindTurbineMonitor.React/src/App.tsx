import { useState } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './hooks/useAuth';
import { Dashboard } from './pages/Dashboard';
import { TurbineDetail } from './pages/TurbineDetail';
import { LoginPage } from './pages/Login';
import './index.css';
import './App.css';

interface ProtectedRouteProps {
  isLoggedIn: boolean;
  children: React.ReactNode;
}

function ProtectedRoute({ isLoggedIn, children }: ProtectedRouteProps) {
  return isLoggedIn ? <>{children}</> : <Navigate to="/login" replace />;
}

function App() {
  const { isLoggedIn, logout } = useAuth();
  const [refreshKey, setRefreshKey] = useState(0);

  const handleLoginSuccess = () => {
    setRefreshKey(prev => prev + 1);
  };

  if (!isLoggedIn) {
    return <LoginPage onLoginSuccess={handleLoginSuccess} />;
  }

  return (
    <div key={refreshKey} className="app-wrapper">
      <nav className="app-navbar">
        <button onClick={logout} className="logout-btn">
          Logout
        </button>
      </nav>

      <BrowserRouter>
        <Routes>
          <Route
            path="/"
            element={
              <ProtectedRoute isLoggedIn={isLoggedIn}>
                <Dashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/turbines/:id"
            element={
              <ProtectedRoute isLoggedIn={isLoggedIn}>
                <TurbineDetail />
              </ProtectedRoute>
            }
          />
          <Route path="/login" element={<LoginPage onLoginSuccess={handleLoginSuccess} />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
