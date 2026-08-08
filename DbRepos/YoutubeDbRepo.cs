using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Models;

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

    public async Task UpdateSocialAccountAsync(Guid id, ISocialAccount update)
    {
        var existing = await _dbContext.SocialAccounts
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing != null)
        {
            existing.AccessToken = update.AccessToken;
            existing.RefreshToken = update.RefreshToken;
            existing.TokenExpires = update.TokenExpires;

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<ISocialAccount?> GetSocialAccountByIdAsync(Guid id)
    {
        return await _dbContext.SocialAccounts
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
