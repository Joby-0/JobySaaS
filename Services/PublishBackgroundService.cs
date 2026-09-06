using DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;

namespace Services;

public sealed class PublishBackgroundService : BackgroundService
{
    private readonly IPublishQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PublishBackgroundService> _logger;

    public PublishBackgroundService(IPublishQueue queue, IServiceScopeFactory scopeFactory, ILogger<PublishBackgroundService> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RequeueRecoverableJobsAsync(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var jobId = await _queue.DequeueAsync(stoppingToken);
                using var scope = _scopeFactory.CreateScope();
                await scope.ServiceProvider.GetRequiredService<IPublishJobProcessor>()
                    .ProcessPublishJobAsync(jobId, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled publish worker error.");
            }
        }
    }

    private async Task RequeueRecoverableJobsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();
        var jobs = await db.PublishJobs
            .Where(x => x.Status == PublishJobStatus.Pending || x.Status == PublishJobStatus.Processing)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        foreach (var jobId in jobs)
            await _queue.EnqueueAsync(jobId, cancellationToken);
    }
}
