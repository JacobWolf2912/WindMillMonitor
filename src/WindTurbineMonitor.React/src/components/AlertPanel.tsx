import type { Alert } from '../types/alert';

interface AlertPanelProps {
  alerts: Alert[];
  onAcknowledge?: (alertId: number) => Promise<void>;
}

export function AlertPanel({ alerts, onAcknowledge }: AlertPanelProps) {
  const severityColors: Record<string, string> = {
    'Critical': 'border-l-4 border-red-500 bg-red-50',
    'Warning': 'border-l-4 border-yellow-500 bg-yellow-50',
    'Info': 'border-l-4 border-blue-500 bg-blue-50',
  };

  const severityBadgeColors: Record<string, string> = {
    'Critical': 'bg-red-100 text-red-800',
    'Warning': 'bg-yellow-100 text-yellow-800',
    'Info': 'bg-blue-100 text-blue-800',
  };

  const handleAcknowledge = async (alertId: number) => {
    if (onAcknowledge) {
      try {
        await onAcknowledge(alertId);
      } catch (err) {
        console.error('Failed to acknowledge alert:', err);
      }
    }
  };

  const unacknowledged = alerts.filter((a) => !a.isAcknowledged);

  return (
    <div className="bg-white p-6 rounded-lg shadow">
      <h2 className="text-xl font-bold mb-4">Alerts</h2>
      {unacknowledged.length === 0 ? (
        <p className="text-gray-500">No active alerts</p>
      ) : (
        <div className="space-y-3">
          {unacknowledged.map((alert) => (
            <div
              key={alert.id}
              className={`p-4 rounded ${severityColors[alert.severity] || 'bg-gray-50'}`}
            >
              <div className="flex justify-between items-start mb-2">
                <div>
                  <span className={`inline-block px-2 py-1 text-xs font-semibold rounded ${severityBadgeColors[alert.severity] || 'bg-gray-100'}`}>
                    {alert.severity}
                  </span>
                  <h4 className="font-semibold mt-2">{alert.title}</h4>
                </div>
                <button
                  onClick={() => handleAcknowledge(alert.id)}
                  className="text-sm px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600"
                >
                  Ack
                </button>
              </div>
              <p className="text-sm text-gray-700">{alert.description}</p>
              <p className="text-xs text-gray-500 mt-2">
                {new Date(alert.timestamp).toLocaleString()}
              </p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
