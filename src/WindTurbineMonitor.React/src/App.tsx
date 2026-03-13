import { useState } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './hooks/useAuth';
import { Dashboard } from './pages/Dashboard';
import { TurbineDetail } from './pages/TurbineDetail';
import { LoginPage } from './pages/Login';
import './index.css';

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
    <div key={refreshKey} className="min-h-screen bg-gray-100">
      <nav className="bg-blue-900 text-white p-4 shadow">
        <div className="max-w-7xl mx-auto flex justify-between items-center">
          <h1 className="text-2xl font-bold">Wind Turbine Monitor</h1>
          <button
            onClick={logout}
            className="bg-red-600 hover:bg-red-700 px-4 py-2 rounded"
          >
            Logout
          </button>
        </div>
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
