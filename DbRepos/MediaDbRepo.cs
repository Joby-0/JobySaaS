using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;

namespace DbRepos;

public class MediaDbRepo
{
    private readonly MainDbContext _dbContext;

    public MediaDbRepo(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MediaDbM> CreateAsync(MediaDbM media)
    {
        _dbContext.Media.Add(media);
        await _dbContext.SaveChangesAsync();
        return media;
    }

    public Task<MediaDbM?> GetByIdAsync(Guid organizationId, Guid mediaId)
    {
        return _dbContext.Media
            .AsNoTracking()
            .FirstOrDefaultAsync(media => media.OrganizationId == organizationId && media.Id == mediaId);
    }

    public Task<List<MediaDbM>> GetMediaListAsync(Guid organizationId, int pageNumber, int pageSize)
    {
        return _dbContext.Media
            .AsNoTracking()
            .Where(media => media.OrganizationId == organizationId)
            .Include(media => media.SocialVideoDbMs)
            .OrderByDescending(media => media.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<MediaDbM?> GetMediaDetailsAsync(Guid organizationId, Guid mediaId)
    {
        return _dbContext.Media
            .AsNoTracking()
            .Where(media => media.OrganizationId == organizationId && media.Id == mediaId)
            .Include(media => media.SocialVideoDbMs)
            .Include(media => media.OrganizationDbM)
                .ThenInclude(organization => organization.SocialAccountDbMs)
            .FirstOrDefaultAsync();
    }
}
