import { useState } from 'react';
import { useCommands } from '../hooks/useCommands';

interface CommandPanelProps {
  turbineId: string;
}

export function CommandPanel({ turbineId }: CommandPanelProps) {
  const { commands, issueCommand } = useCommands(turbineId);
  const [username, setUsername] = useState('operator');
  const [targetRpm, setTargetRpm] = useState(15);
  const [isLoading, setIsLoading] = useState(false);
  const [feedback, setFeedback] = useState<{ type: 'success' | 'error'; message: string } | null>(null);

  const handleCommand = async (type: string, rpm?: number) => {
    try {
      setIsLoading(true);
      setFeedback(null);
      await issueCommand(type, rpm, username);
      setFeedback({ type: 'success', message: `${type} command issued` });
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Command failed';
      setFeedback({ type: 'error', message });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="bg-white rounded-xl shadow-lg overflow-hidden">
      <div className="bg-gradient-to-r from-purple-50 to-purple-100 px-8 py-6 border-b border-purple-200">
        <h2 className="text-xl font-bold text-gray-900">🎛️ Control Commands</h2>
      </div>

      <div className="p-8 space-y-6">
        <div>
          <label className="block text-sm font-semibold text-gray-700 mb-3">Operator Name</label>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            className="w-full px-4 py-2 border-2 border-gray-300 rounded-lg text-gray-900 focus:border-purple-500 focus:outline-none transition"
            placeholder="operator"
          />
        </div>

        <div className="space-y-3">
          <p className="text-sm font-semibold text-gray-700">Quick Commands</p>
          <div className="grid grid-cols-2 gap-3">
            <button
              onClick={() => handleCommand('Start')}
              disabled={isLoading}
              className="bg-gradient-to-r from-green-500 to-green-600 hover:from-green-600 hover:to-green-700 disabled:from-gray-400 disabled:to-gray-500 text-white font-bold py-3 px-4 rounded-lg shadow-md hover:shadow-lg transition transform hover:scale-105"
            >
              ▶ Start
            </button>
            <button
              onClick={() => handleCommand('Stop')}
              disabled={isLoading}
              className="bg-gradient-to-r from-yellow-500 to-yellow-600 hover:from-yellow-600 hover:to-yellow-700 disabled:from-gray-400 disabled:to-gray-500 text-white font-bold py-3 px-4 rounded-lg shadow-md hover:shadow-lg transition transform hover:scale-105"
            >
              ⏸ Stop
            </button>
          </div>
          <button
            onClick={() => handleCommand('EmergencyStop')}
            disabled={isLoading}
            className="w-full bg-gradient-to-r from-red-500 to-red-600 hover:from-red-600 hover:to-red-700 disabled:from-gray-400 disabled:to-gray-500 text-white font-bold py-3 px-4 rounded-lg shadow-lg hover:shadow-xl transition transform hover:scale-105"
          >
            🛑 EMERGENCY STOP
          </button>
        </div>

        <div className="bg-gray-50 border border-gray-200 rounded-lg p-4">
          <label className="block text-sm font-semibold text-gray-700 mb-3">Set Target RPM</label>
          <div className="flex gap-3">
            <input
              type="number"
              min="0"
              max="50"
              value={targetRpm}
              onChange={(e) => setTargetRpm(parseFloat(e.target.value) || 0)}
              className="flex-1 px-4 py-2 border-2 border-gray-300 rounded-lg text-gray-900 focus:border-purple-500 focus:outline-none transition"
              placeholder="RPM value"
            />
            <button
              onClick={() => handleCommand('SetTargetRpm', targetRpm)}
              disabled={isLoading}
              className="bg-gradient-to-r from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700 disabled:from-gray-400 disabled:to-gray-500 text-white font-bold py-2 px-6 rounded-lg shadow-md hover:shadow-lg transition"
            >
              ⚙️ Set
            </button>
          </div>
        </div>

        {feedback && (
          <div
            className={`p-4 rounded-lg text-sm font-medium ${
              feedback.type === 'success'
                ? 'bg-green-100 text-green-800 border border-green-300'
                : 'bg-red-100 text-red-800 border border-red-300'
            }`}
          >
            {feedback.type === 'success' ? '✓ ' : '✗ '}{feedback.message}
          </div>
        )}
      </div>

      {commands.length > 0 && (
        <div className="border-t border-gray-200">
          <div className="bg-gradient-to-r from-gray-50 to-gray-100 px-8 py-4 border-b border-gray-200">
            <h3 className="text-lg font-semibold text-gray-900">📋 Command History</h3>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="bg-gray-50 border-b border-gray-200">
                  <th className="text-left py-3 px-6 font-semibold text-gray-700">Command</th>
                  <th className="text-left py-3 px-6 font-semibold text-gray-700">Operator</th>
                  <th className="text-left py-3 px-6 font-semibold text-gray-700">Time</th>
                  <th className="text-left py-3 px-6 font-semibold text-gray-700">Status</th>
                </tr>
              </thead>
              <tbody>
                {commands.slice(0, 10).map((cmd) => (
                  <tr key={cmd.id} className="border-b border-gray-100 hover:bg-gray-50 transition">
                    <td className="py-3 px-6 font-medium text-gray-900">{cmd.commandType}</td>
                    <td className="py-3 px-6 text-gray-700">{cmd.issuedByUsername}</td>
                    <td className="py-3 px-6 text-gray-600 text-xs">
                      {new Date(cmd.issuedAt).toLocaleTimeString()}
                    </td>
                    <td className="py-3 px-6">
                      <span
                        className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${
                          cmd.status === 'Executed'
                            ? 'bg-green-100 text-green-800'
                            : cmd.status === 'Pending'
                            ? 'bg-yellow-100 text-yellow-800'
                            : 'bg-red-100 text-red-800'
                        }`}
                      >
                        {cmd.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
