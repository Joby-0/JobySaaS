using DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;

namespace Services;

public interface IPublishJobProcessor
{
    Task ProcessPublishJobAsync(Guid publishJobId, CancellationToken cancellationToken);
}

public sealed class PublishJobProcessor : IPublishJobProcessor
{
    private readonly MainDbContext _db;
    private readonly IReadOnlyDictionary<SocialPlatform, ISocialMediaPublisher> _publishers;
    private readonly ILogger<PublishJobProcessor> _logger;

    public PublishJobProcessor(MainDbContext db, IEnumerable<ISocialMediaPublisher> publishers, ILogger<PublishJobProcessor> logger)
    {
        _db = db;
        _publishers = publishers.ToDictionary(x => x.Platform);
        _logger = logger;
    }

    public async Task ProcessPublishJobAsync(Guid publishJobId, CancellationToken cancellationToken)
    {
        var job = await _db.PublishJobs
            .Include(x => x.Accounts)
                .ThenInclude(x => x.SocialAccount)
                    .ThenInclude(x => x.OrganizationDbM)
            .Include(x => x.MediaDbM)
            .SingleOrDefaultAsync(x => x.Id == publishJobId, cancellationToken);

        if (job is null || job.Status is PublishJobStatus.Completed or PublishJobStatus.CompletedWithErrors)
            return;

        job.Status = PublishJobStatus.Processing;
        job.StartedAt ??= DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        foreach (var jobAccount in job.Accounts.Where(x => x.Status is PublishJobAccountStatus.Pending or PublishJobAccountStatus.Processing))
        {
            cancellationToken.ThrowIfCancellationRequested();
            jobAccount.Status = PublishJobAccountStatus.Processing;
            jobAccount.StartedAt ??= DateTime.UtcNow;
            jobAccount.ErrorMessage = null;
            await _db.SaveChangesAsync(cancellationToken);

            try
            {
                if (!_publishers.TryGetValue(jobAccount.SocialAccount.Platform, out var publisher))
                {
                    throw new InvalidOperationException($"Publishing for {jobAccount.SocialAccount.Platform} is not configured.");
                }

                var result = await publisher.PublishAsync(jobAccount.SocialAccount, job.MediaDbM, cancellationToken);
                if (!result.Success)
                    throw new InvalidOperationException(result.ErrorMessage ?? "Publishing failed.");

                jobAccount.Status = PublishJobAccountStatus.Completed;
                jobAccount.ExternalPostId = result.ExternalPostId;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Publishing job account {JobAccountId} failed.", jobAccount.Id);
                jobAccount.Status = PublishJobAccountStatus.Failed;
                jobAccount.ErrorMessage = ex.Message;
            }

            jobAccount.CompletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
        }

        var failed = job.Accounts.Any(x => x.Status == PublishJobAccountStatus.Failed);
        job.Status = failed
            ? PublishJobStatus.CompletedWithErrors
            : PublishJobStatus.Completed;
        job.CompletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
