import { useTurbines } from '../hooks/useTurbines';
import { useLatestMetric } from '../hooks/useMetrics';
import { useAlerts } from '../hooks/useAlerts';
import { TurbineCard } from '../components/TurbineCard';
import { AlertPanel } from '../components/AlertPanel';
import './Dashboard.css';

export function Dashboard() {
  const { turbines, loading: turbinesLoading } = useTurbines();
  const { alerts, loading: alertsLoading, acknowledge } = useAlerts();

  return (
    <div className="dashboard-container">
      <header className="dashboard-header">
        <h1>💨 Wind Turbine Monitor</h1>
        <p>Windmill Inspection Centre</p>
      </header>

      <main className="dashboard-main">
        <div className="dashboard-grid">
          {/* Main turbines section */}
          <div className="turbines-section">
            <h2>Fleet Overview</h2>
            <p className="section-description">Manage and monitor your wind turbines</p>

            {turbinesLoading ? (
              <div className="loading-box">Loading turbines...</div>
            ) : turbines.length === 0 ? (
              <div className="empty-state">
                <p>📡 No turbines connected</p>
                <p>Publish MQTT metrics to <code>broker.hivemq.com</code> to register turbines.</p>
              </div>
            ) : (
              <div className="turbines-grid">
                {turbines.map((turbine) => (
                  <TurbineCardWithMetric key={turbine.id} turbineId={turbine.id} turbine={turbine} />
                ))}
              </div>
            )}
          </div>

          {/* Alerts sidebar */}
          <div className="alerts-section">
            <h2>Alerts</h2>
            <p className="section-description">System status and notifications</p>

            {alertsLoading ? (
              <div className="loading-box">Loading alerts...</div>
            ) : (
              <AlertPanel alerts={alerts} onAcknowledge={acknowledge} />
            )}
          </div>
        </div>
      </main>
    </div>
  );
}

function TurbineCardWithMetric({ turbineId, turbine }: { turbineId: number; turbine: any }) {
  const { metric } = useLatestMetric(turbineId);
  return <TurbineCard turbine={turbine} metric={metric || null} />;
}
