using System.Collections.Concurrent;
using System.Threading.Channels;
using WindTurbineMonitor.Api.Dtos;

namespace WindTurbineMonitor.Api.Services;

public class MetricBroadcaster
{
    private readonly ConcurrentDictionary<string, Channel<MetricDto>> _channels = new();

    public ChannelReader<MetricDto> Subscribe(string turbineId)
    {
        var channel = _channels.GetOrAdd(turbineId, _ => Channel.CreateUnbounded<MetricDto>());
        return channel.Reader;
    }

    public async Task PublishAsync(string turbineId, MetricDto metric)
    {
        if (_channels.TryGetValue(turbineId, out var channel))
        {
            await channel.Writer.WriteAsync(metric);
        }
    }
}
