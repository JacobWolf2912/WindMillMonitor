import { useState } from 'react';
import { useCommands } from '../hooks/useCommands';

interface CommandPanelProps {
  turbineId: number;
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
    <div className="bg-white p-6 rounded-lg shadow">
      <h2 className="text-xl font-bold mb-4">Control Commands</h2>

      <div className="space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">Username</label>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md text-gray-900"
            placeholder="operator"
          />
        </div>

        <div className="grid grid-cols-2 gap-2">
          <button
            onClick={() => handleCommand('Start')}
            disabled={isLoading}
            className="bg-green-600 hover:bg-green-700 disabled:bg-gray-400 text-white font-bold py-2 px-4 rounded"
          >
            Start
          </button>
          <button
            onClick={() => handleCommand('Stop')}
            disabled={isLoading}
            className="bg-yellow-600 hover:bg-yellow-700 disabled:bg-gray-400 text-white font-bold py-2 px-4 rounded"
          >
            Stop
          </button>
          <button
            onClick={() => handleCommand('EmergencyStop')}
            disabled={isLoading}
            className="bg-red-600 hover:bg-red-700 disabled:bg-gray-400 text-white font-bold py-2 px-4 rounded col-span-2"
          >
            Emergency Stop
          </button>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">Set Target RPM</label>
          <div className="flex gap-2">
            <input
              type="number"
              min="0"
              max="50"
              value={targetRpm}
              onChange={(e) => setTargetRpm(parseFloat(e.target.value) || 0)}
              className="flex-1 px-3 py-2 border border-gray-300 rounded-md text-gray-900"
            />
            <button
              onClick={() => handleCommand('SetTargetRpm', targetRpm)}
              disabled={isLoading}
              className="bg-blue-600 hover:bg-blue-700 disabled:bg-gray-400 text-white font-bold py-2 px-4 rounded"
            >
              Set
            </button>
          </div>
        </div>

        {feedback && (
          <div
            className={`p-3 rounded text-sm ${
              feedback.type === 'success'
                ? 'bg-green-100 text-green-800'
                : 'bg-red-100 text-red-800'
            }`}
          >
            {feedback.message}
          </div>
        )}
      </div>

      <div className="mt-6">
        <h3 className="text-lg font-semibold mb-3">Command History</h3>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b">
                <th className="text-left py-2 px-2">Type</th>
                <th className="text-left py-2 px-2">User</th>
                <th className="text-left py-2 px-2">Time</th>
                <th className="text-left py-2 px-2">Status</th>
              </tr>
            </thead>
            <tbody>
              {commands.slice(0, 10).map((cmd) => (
                <tr key={cmd.id} className="border-b hover:bg-gray-50">
                  <td className="py-2 px-2 font-medium">{cmd.commandType}</td>
                  <td className="py-2 px-2">{cmd.issuedByUsername}</td>
                  <td className="py-2 px-2 text-gray-600 text-xs">
                    {new Date(cmd.issuedAt).toLocaleTimeString()}
                  </td>
                  <td className="py-2 px-2">
                    <span
                      className={`inline-block px-2 py-1 rounded text-xs font-medium ${
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
          {commands.length === 0 && <p className="text-gray-500 text-sm py-4">No commands yet</p>}
        </div>
      </div>
    </div>
  );
}
