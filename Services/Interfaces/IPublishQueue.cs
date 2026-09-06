namespace Services;

public interface IPublishQueue
{
    ValueTask EnqueueAsync(Guid publishJobId, CancellationToken cancellationToken = default);
    ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken);
}
