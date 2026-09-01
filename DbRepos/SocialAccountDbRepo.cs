using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DbRepos;

public class SocialAccountDbRepo
{
    private readonly MainDbContext _dbContext;

    public SocialAccountDbRepo(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SocialAccountDbM>> GetByOrganizationIdAsync(Guid organizationId)
    {
        return await _dbContext.SocialAccounts.AsNoTracking().Where(x => x.OrganizationId == organizationId).ToListAsync();
    }

    public async Task<ISocialAccount?> GetSocialAccountByIdAsync(Guid id)
    {
        return await _dbContext.SocialAccounts.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<string> SaveSocialAccountAsync(SocialAccountDbM account)
    {
        try
        {
            var existing = await _dbContext.SocialAccounts.FirstOrDefaultAsync(x => x.Platform == account.Platform && x.Username == account.Username && x.OrganizationId == account.OrganizationId);

            if (existing != null)
            {
                existing.AccessToken = account.AccessToken;
                existing.TokenExpiresAt = account.TokenExpiresAt;
            }
            else
            {
                account.Id = Guid.NewGuid();
                _dbContext.SocialAccounts.Add(account);
            }

            await _dbContext.SaveChangesAsync();
            return "YouTube account connected successfully.";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            return $"Failed to connect YouTube account: {ex.InnerException?.Message ?? ex.Message}";
        }
    }

    public async Task<bool> DeleteAsync(Guid id, Guid organizationId)
    {
        // scoped by organizationId too, not just id — an org can only delete its own accounts,
        // same "never trust the client-side id alone" principle as everywhere else in this API
        var account = await _dbContext.SocialAccounts.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId);

        if (account is null) return false;

        _dbContext.SocialAccounts.Remove(account);
        await _dbContext.SaveChangesAsync();
        return true;
    }
    public async Task UpdateSocialAccountAsync(Guid id, ISocialAccount update)
    {
        var existing = await _dbContext.SocialAccounts
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing != null)
        {
            existing.AccessToken = update.AccessToken;
            existing.RefreshToken = update.RefreshToken;
            existing.TokenExpiresAt = update.TokenExpiresAt;

            await _dbContext.SaveChangesAsync();
        }
    }
}