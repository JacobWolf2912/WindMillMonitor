import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from 'recharts';
import type { Metric } from '../types/metric';

interface MetricChartProps {
  title: string;
  data: Metric[];
  dataKey: keyof Metric;
  unit?: string;
}

export function MetricChart({ title, data, dataKey, unit }: MetricChartProps) {
  const formatTime = (timestamp: string) => {
    const date = new Date(timestamp);
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  };

  return (
    <div className="bg-white p-4 rounded-lg shadow">
      <h3 className="text-lg font-semibold mb-4">{title}</h3>
      <ResponsiveContainer width="100%" height={300}>
        <AreaChart data={data}>
          <defs>
            <linearGradient id="colorMetric" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#3b82f6" stopOpacity={0.3} />
              <stop offset="95%" stopColor="#3b82f6" stopOpacity={0} />
            </linearGradient>
          </defs>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="timestamp" tickFormatter={formatTime} />
          <YAxis label={{ value: unit, angle: -90, position: 'insideLeft' }} />
          <Tooltip
            formatter={(value) =>
              value
                ? `${Number(value).toFixed(2)} ${unit || ''}`
                : 'N/A'
            }
          />
          <Area
            type="monotone"
            dataKey={dataKey as string}
            stroke="#3b82f6"
            fillOpacity={1}
            fill="url(#colorMetric)"
          />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  );
}
