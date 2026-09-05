using DbContext;
using Microsoft.EntityFrameworkCore;
using Models.DTO;

namespace Services;

public sealed class PublishJobService : IPublishJobService
{
    private readonly MainDbContext _db;
    private readonly DbRepos.OrganizationDbRepo _organizations;

    public PublishJobService(MainDbContext db, DbRepos.OrganizationDbRepo organizations)
    {
        _db = db;
        _organizations = organizations;
    }

    public async Task<ServiceResult<PublishJobDto>> GetStatusAsync(Guid organizationId, Guid jobId, Guid requestUserId)
    {
        if (await _organizations.GetUserOrganizationAsync(organizationId, requestUserId) is null)
            return ServiceResult<PublishJobDto>.Fail("You do not have access to this organization.");

        var job = await _db.PublishJobs.AsNoTracking()
            .Include(x => x.Accounts).ThenInclude(x => x.SocialAccount)
            .SingleOrDefaultAsync(x => x.Id == jobId && x.OrganizationId == organizationId);
        if (job is null)
            return ServiceResult<PublishJobDto>.Fail("Publish job could not be found.");

        return ServiceResult<PublishJobDto>.Ok("Publish job status retrieved.", new PublishJobDto
        {
            Id = job.Id, Status = job.Status, CreatedAt = job.CreatedAt,
            StartedAt = job.StartedAt, CompletedAt = job.CompletedAt,
            Accounts = job.Accounts.Select(x => new PublishJobAccountDto
            {
                SocialAccountId = x.SocialAccountId,
                Platform = x.SocialAccount.Platform,
                Status = x.Status, ExternalPostId = x.ExternalPostId,
                ErrorMessage = x.ErrorMessage, CreatedAt = x.CreatedAt,
                StartedAt = x.StartedAt, CompletedAt = x.CompletedAt
            }).ToList()
        });
    }
}
