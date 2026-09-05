using System.Threading.Channels;

namespace Services;

public sealed class PublishQueue : IPublishQueue
{
    private readonly Channel<Guid> _jobs = Channel.CreateBounded<Guid>(new BoundedChannelOptions(1000)
    {
        FullMode = BoundedChannelFullMode.Wait,
        SingleReader = true,
        SingleWriter = false
    });

    public ValueTask EnqueueAsync(Guid publishJobId, CancellationToken cancellationToken = default) =>
        _jobs.Writer.WriteAsync(publishJobId, cancellationToken);

    public ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken) =>
        _jobs.Reader.ReadAsync(cancellationToken);
}
