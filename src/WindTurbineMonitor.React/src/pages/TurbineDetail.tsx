import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTurbines } from '../hooks/useTurbines';
import { useMetrics, useLatestMetric } from '../hooks/useMetrics';
import { useAlerts } from '../hooks/useAlerts';
import { useSse } from '../hooks/useSse';
import { MetricChart } from '../components/MetricChart';
import { StatusBadge } from '../components/StatusBadge';
import { AlertPanel } from '../components/AlertPanel';
import { CommandPanel } from '../components/CommandPanel';
import type { Metric } from '../types/metric';

export function TurbineDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const turbineId = parseInt(id || '0', 10);

  const { turbines } = useTurbines();
  const { metrics } = useMetrics(turbineId);
  const { metric: latestMetric } = useLatestMetric(turbineId);
  const { alerts, acknowledge } = useAlerts(turbineId);
  const [liveMetric, setLiveMetric] = useState<Metric | null>(latestMetric || null);

  useSse<Metric>(`/sse/turbines/${turbineId}`, (data) => {
    setLiveMetric(data);
  });

  const turbine = turbines.find((t) => t.id === turbineId);
  const displayMetric = liveMetric || latestMetric;

  if (!turbine) {
    return (
      <div className="min-h-screen bg-gray-100 p-6">
        <button
          onClick={() => navigate('/')}
          className="text-blue-600 hover:text-blue-800 mb-4"
        >
          ← Back to Dashboard
        </button>
        <p className="text-gray-600">Turbine not found.</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-100">
      <div className="bg-blue-600 text-white p-6">
        <button
          onClick={() => navigate('/')}
          className="text-blue-100 hover:text-white mb-4"
        >
          ← Back to Dashboard
        </button>
        <div className="flex justify-between items-start">
          <div>
            <h1 className="text-3xl font-bold">{turbine.name}</h1>
            <p className="text-blue-100">{turbine.location}</p>
          </div>
          {displayMetric && <StatusBadge status={displayMetric.status} />}
        </div>
      </div>

      <div className="container mx-auto p-6">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div className="lg:col-span-2 space-y-6">
            {metrics && metrics.length > 0 ? (
              <>
                <MetricChart
                  title="RPM & Power"
                  data={metrics}
                  dataKey="rotorRpm"
                  unit="RPM"
                />
                <MetricChart
                  title="Wind Metrics"
                  data={metrics}
                  dataKey="windSpeedMs"
                  unit="m/s"
                />
                <MetricChart
                  title="Temperature"
                  data={metrics}
                  dataKey="nacelleTemperatureCelsius"
                  unit="°C"
                />
              </>
            ) : (
              <p className="text-gray-600">No historical metrics available.</p>
            )}
          </div>

          <div className="space-y-6">
            <div className="bg-white p-6 rounded-lg shadow">
              <h2 className="text-xl font-bold mb-4">Live Reading</h2>
              {displayMetric ? (
                <div className="space-y-3">
                  <div>
                    <p className="text-sm text-gray-600">Rotor RPM</p>
                    <p className="text-2xl font-bold">{displayMetric.rotorRpm?.toFixed(1) || 'N/A'}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">Power Output</p>
                    <p className="text-2xl font-bold">{displayMetric.powerOutputKw?.toFixed(0) || 'N/A'} kW</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">Wind Speed</p>
                    <p className="text-2xl font-bold">{displayMetric.windSpeedMs?.toFixed(1) || 'N/A'} m/s</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">Nacelle Temp</p>
                    <p className="text-2xl font-bold">{displayMetric.nacelleTemperatureCelsius?.toFixed(1) || 'N/A'} °C</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">Gearbox Temp</p>
                    <p className="text-2xl font-bold">{displayMetric.gearboxTemperatureCelsius?.toFixed(1) || 'N/A'} °C</p>
                  </div>
                  <div className="pt-4 border-t">
                    <p className="text-sm text-gray-600">Status</p>
                    <StatusBadge status={displayMetric.status} />
                  </div>
                </div>
              ) : (
                <p className="text-gray-500">No data available</p>
              )}
            </div>
            <AlertPanel alerts={alerts} onAcknowledge={acknowledge} />
            <CommandPanel turbineId={turbineId} />
          </div>
        </div>
      </div>
    </div>
  );
}
