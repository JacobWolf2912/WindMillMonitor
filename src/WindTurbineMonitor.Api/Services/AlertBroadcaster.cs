using System.Collections.Concurrent;
using System.Threading.Channels;
using WindTurbineMonitor.Api.Dtos;

namespace WindTurbineMonitor.Api.Services;

public class AlertBroadcaster
{
    private readonly Channel<AlertDto> _channel = Channel.CreateUnbounded<AlertDto>();

    public ChannelReader<AlertDto> Subscribe()
    {
        return _channel.Reader;
    }

    public async Task PublishAsync(AlertDto alert)
    {
        await _channel.Writer.WriteAsync(alert);
    }
}
