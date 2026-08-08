using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;

namespace DbRepos;

public class YoutubeDbRepo
{
    readonly ReferenceDbContext _dbContext;

    public YoutubeDbRepo(ReferenceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveSocialAccountAsync(SocialAccountDbM account)
    {
        var existing = await _dbContext.SocialAccounts
            .FirstOrDefaultAsync(x => x.Platform == "YouTube" && x.Username == account.Username && x.OrganizationId == account.OrganizationId);

        if (existing != null)
        {
            existing.AccessToken = account.AccessToken;
            existing.TokenExpires = account.TokenExpires;
        }
        else
        {
            _dbContext.SocialAccounts.Add(account);
        }

        await _dbContext.SaveChangesAsync();
    }
}
