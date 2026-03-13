import { useTurbines } from '../hooks/useTurbines';
import { useLatestMetric } from '../hooks/useMetrics';
import { useAlerts } from '../hooks/useAlerts';
import { TurbineCard } from '../components/TurbineCard';
import { AlertPanel } from '../components/AlertPanel';

export function Dashboard() {
  const { turbines, loading: turbinesLoading } = useTurbines();
  const { alerts, loading: alertsLoading, acknowledge } = useAlerts();

  return (
    <div className="min-h-screen bg-gray-100">
      <header className="bg-blue-600 text-white p-6">
        <h1 className="text-3xl font-bold">🌬️ Wind Turbine Monitor</h1>
        <p className="text-blue-100">FS+IoT Corporate™ - Windmill Inspection Centre</p>
      </header>

      <div className="container mx-auto p-6">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div className="lg:col-span-2">
            <h2 className="text-2xl font-bold mb-4">Turbines</h2>
            {turbinesLoading ? (
              <p className="text-gray-600">Loading turbines...</p>
            ) : turbines.length === 0 ? (
              <p className="text-gray-600">No turbines available. Publish MQTT metrics to register turbines.</p>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {turbines.map((turbine) => (
                  <TurbineCardWithMetric key={turbine.id} turbineId={turbine.id} turbine={turbine} />
                ))}
              </div>
            )}
          </div>

          <div>
            {alertsLoading ? (
              <p className="text-gray-600">Loading alerts...</p>
            ) : (
              <AlertPanel alerts={alerts} onAcknowledge={acknowledge} />
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

function TurbineCardWithMetric({ turbineId, turbine }: { turbineId: number; turbine: any }) {
  const { metric } = useLatestMetric(turbineId);
  return <TurbineCard turbine={turbine} metric={metric || null} />;
}
