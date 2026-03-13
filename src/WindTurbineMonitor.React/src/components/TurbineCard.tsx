import { Link } from 'react-router-dom';
import type { Turbine } from '../types/turbine';
import type { Metric } from '../types/metric';
import { StatusBadge } from './StatusBadge';

interface TurbineCardProps {
  turbine: Turbine;
  metric: Metric | null;
}

export function TurbineCard({ turbine, metric }: TurbineCardProps) {
  return (
    <Link to={`/turbines/${turbine.id}`}>
      <div className="bg-white p-6 rounded-lg shadow hover:shadow-lg transition cursor-pointer">
        <div className="flex justify-between items-start mb-4">
          <div>
            <h3 className="text-xl font-bold">{turbine.name}</h3>
            <p className="text-sm text-gray-600">{turbine.location}</p>
          </div>
          {metric && <StatusBadge status={metric.status} />}
        </div>

        {metric ? (
          <div className="space-y-2 text-sm">
            <div className="flex justify-between">
              <span className="text-gray-600">RPM:</span>
              <span className="font-semibold">{metric.rotorRpm?.toFixed(1) || 'N/A'}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Power:</span>
              <span className="font-semibold">{metric.powerOutputKw?.toFixed(0) || 'N/A'} kW</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Wind:</span>
              <span className="font-semibold">{metric.windSpeedMs?.toFixed(1) || 'N/A'} m/s</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Temp:</span>
              <span className="font-semibold">{metric.nacelleTemperatureCelsius?.toFixed(1) || 'N/A'} °C</span>
            </div>
          </div>
        ) : (
          <p className="text-gray-500 text-sm">No data available</p>
        )}
      </div>
    </Link>
  );
}
