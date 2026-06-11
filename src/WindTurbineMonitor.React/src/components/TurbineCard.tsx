import { Link } from 'react-router-dom';
import type { Turbine } from '../types/turbine';
import type { Metric } from '../types/metric';
import { StatusBadge } from './StatusBadge';
import './TurbineCard.css';

interface TurbineCardProps {
  turbine: Turbine;
  metric: Metric | null;
}

export function TurbineCard({ turbine, metric }: TurbineCardProps) {
  return (
    <Link to={`/turbines/${turbine.id}`} className="turbine-card-link">
      <div className="turbine-card">
        <div className="turbine-card-header">
          <div>
            <h3 className="turbine-name">{turbine.name}</h3>
            <p className="turbine-location">📍 {turbine.location}</p>
          </div>
          {metric && <StatusBadge status={metric.status} />}
        </div>

        {metric ? (
          <div className="metrics-grid">
            <div className="metric-item">
              <span className="metric-label">RPM</span>
              <span className="metric-value">{metric.rotorRpm?.toFixed(1) || 'N/A'}</span>
            </div>
            <div className="metric-item">
              <span className="metric-label">Power</span>
              <span className="metric-value">{metric.powerOutputKw?.toFixed(0) || 'N/A'} kW</span>
            </div>
            <div className="metric-item">
              <span className="metric-label">Wind</span>
              <span className="metric-value">{metric.windSpeedMs?.toFixed(1) || 'N/A'} m/s</span>
            </div>
            <div className="metric-item">
              <span className="metric-label">Temp</span>
              <span className="metric-value">{metric.nacelleTemperatureCelsius?.toFixed(1) || 'N/A'}°C</span>
            </div>
          </div>
        ) : (
          <div className="no-data">No data available yet</div>
        )}
      </div>
    </Link>
  );
}
