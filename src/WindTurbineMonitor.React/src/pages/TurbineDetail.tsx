import { useState, useRef, useEffect, useCallback } from 'react';
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
  const turbineId = id || '';

  console.log('TurbineDetail - id from params:', id, 'turbineId:', turbineId);

  const { turbines } = useTurbines();
  const { metrics } = useMetrics(turbineId);
  const { metric: latestMetric } = useLatestMetric(turbineId);
  const { alerts, acknowledge } = useAlerts(turbineId);
  const [liveMetric, setLiveMetric] = useState<Metric | null>(latestMetric || null);
  const lastUpdateRef = useRef<number>(0);

  const handleSseUpdate = useCallback((data: Metric) => {
    const now = Date.now();
    if (now - lastUpdateRef.current >= 15000) {
      setLiveMetric(data);
      lastUpdateRef.current = now;
    }
  }, []);

  useSse<Metric>(`/sse/turbines/${turbineId}`, handleSseUpdate);

  const turbine = turbines.find((t) => t.id === turbineId);
  const displayMetric = liveMetric || latestMetric;
  const chartData = liveMetric && metrics
    ? [liveMetric, ...metrics.filter(m => m.timestamp !== liveMetric.timestamp)]
    : metrics;

  useEffect(() => {
    console.log('Chart data:', { liveMetric, metricsLength: metrics?.length, chartDataLength: chartData?.length });
  }, [chartData, liveMetric, metrics]);

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
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100">
      <div className="bg-gradient-to-r from-blue-900 to-blue-800 text-white border-b-4 border-blue-600 shadow-lg">
        <div className="container mx-auto px-6 py-8">
          <button
            onClick={() => navigate('/')}
            className="text-blue-200 hover:text-white mb-6 font-medium flex items-center gap-2 transition-colors"
          >
            ← Back to Dashboard
          </button>
          <div className="flex justify-between items-start">
            <div>
              <h1 className="text-4xl font-bold mb-2">{turbine.name}</h1>
              <p className="text-blue-100 text-lg">{turbine.location}</p>
            </div>
            {displayMetric && <StatusBadge status={displayMetric.status} />}
          </div>
        </div>
      </div>

      <div className="container mx-auto px-6 py-10">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2 space-y-8">
            <div className="bg-white rounded-xl shadow-lg overflow-hidden">
              <div className="bg-gradient-to-r from-indigo-50 to-indigo-100 px-8 py-6 border-b border-indigo-200">
                <h2 className="text-2xl font-bold text-gray-900">📈 Historical Data</h2>
              </div>
              <div className="p-8">
                {metrics && metrics.length > 0 ? (
                  <div className="space-y-8">
                    <div className="bg-white border border-gray-200 rounded-lg p-6 hover:shadow-md transition-shadow">
                      <MetricChart
                        title="RPM & Power"
                        data={chartData}
                        dataKey="rotorRpm"
                        unit="RPM"
                      />
                    </div>
                    <div className="bg-white border border-gray-200 rounded-lg p-6 hover:shadow-md transition-shadow">
                      <MetricChart
                        title="Wind Metrics"
                        data={chartData}
                        dataKey="windSpeedMs"
                        unit="m/s"
                      />
                    </div>
                    <div className="bg-white border border-gray-200 rounded-lg p-6 hover:shadow-md transition-shadow">
                      <MetricChart
                        title="Generator Temperature"
                        data={chartData}
                        dataKey="generatorTemperatureCelsius"
                        unit="°C"
                      />
                    </div>
                  </div>
                ) : (
                  <div className="text-center py-12">
                    <p className="text-gray-500 text-lg">⏳ No historical metrics available yet</p>
                  </div>
                )}
              </div>
            </div>
          </div>

          <div className="space-y-8">
            <div className="bg-white rounded-xl shadow-lg border-t-4 border-blue-500 overflow-hidden">
              <div className="bg-gradient-to-r from-blue-50 to-blue-100 px-8 py-6 border-b border-blue-200">
                <h2 className="text-xl font-bold text-gray-900">📊 Live Reading</h2>
              </div>
              <div className="p-8">
                {displayMetric ? (
                  <div className="space-y-4">
                    <div className="grid grid-cols-2 gap-4 mb-6">
                      <div className="bg-gradient-to-br from-blue-50 to-blue-100 rounded-lg p-4 border border-blue-200">
                        <p className="text-xs font-semibold text-gray-600 uppercase tracking-wide">Rotor RPM</p>
                        <p className="text-3xl font-bold text-blue-900 mt-2">{displayMetric.rotorRpm?.toFixed(1) || 'N/A'}</p>
                      </div>
                      <div className="bg-gradient-to-br from-green-50 to-green-100 rounded-lg p-4 border border-green-200">
                        <p className="text-xs font-semibold text-gray-600 uppercase tracking-wide">Power Output</p>
                        <p className="text-3xl font-bold text-green-900 mt-2">{displayMetric.powerOutputKw?.toFixed(0) || 'N/A'} <span className="text-lg">kW</span></p>
                      </div>
                      <div className="bg-gradient-to-br from-cyan-50 to-cyan-100 rounded-lg p-4 border border-cyan-200">
                        <p className="text-xs font-semibold text-gray-600 uppercase tracking-wide">Wind Speed</p>
                        <p className="text-3xl font-bold text-cyan-900 mt-2">{displayMetric.windSpeedMs?.toFixed(1) || 'N/A'} <span className="text-lg">m/s</span></p>
                      </div>
                      <div className="bg-gradient-to-br from-orange-50 to-orange-100 rounded-lg p-4 border border-orange-200">
                        <p className="text-xs font-semibold text-gray-600 uppercase tracking-wide">Nacelle Temp</p>
                        <p className="text-3xl font-bold text-orange-900 mt-2">{displayMetric.nacelleTemperatureCelsius?.toFixed(1) || 'N/A'}° <span className="text-lg">C</span></p>
                      </div>
                    </div>
                    <div className="grid grid-cols-1 gap-4">
                      <div className="bg-gradient-to-br from-red-50 to-red-100 rounded-lg p-4 border border-red-200">
                        <p className="text-xs font-semibold text-gray-600 uppercase tracking-wide">Gearbox Temp</p>
                        <p className="text-3xl font-bold text-red-900 mt-2">{displayMetric.gearboxTemperatureCelsius?.toFixed(1) || 'N/A'}° <span className="text-lg">C</span></p>
                      </div>
                      <div className="bg-gradient-to-r from-gray-50 to-gray-100 rounded-lg p-4 border border-gray-200 flex items-center justify-between">
                        <p className="text-sm font-semibold text-gray-700 uppercase tracking-wide">System Status</p>
                        <StatusBadge status={displayMetric.status} />
                      </div>
                    </div>
                  </div>
                ) : (
                  <div className="text-center py-12">
                    <p className="text-gray-500 text-lg">⏳ No data available yet</p>
                  </div>
                )}
              </div>
            </div>
            <AlertPanel alerts={alerts} onAcknowledge={acknowledge} />
            <CommandPanel turbineId={turbineId} />
          </div>
        </div>
      </div>
    </div>
  );
}
