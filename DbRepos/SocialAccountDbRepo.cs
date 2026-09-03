using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;

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

    public async Task<SocialAccountDbM?> GetSocialAccountByIdAsync(Guid id)
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
    public async Task DisconnectAccountAsync(Guid accountId, Guid organizationId)
    {
        var account = await _dbContext.SocialAccounts.FirstOrDefaultAsync(x => x.Id == accountId && x.OrganizationId == organizationId);

        if (account is null) return;

        account.Status = SocialAccountStatus.Disconnected;
        account.AccessToken = null;
        account.RefreshToken = null;
        account.TokenExpiresAt = null;
        account.LastSync = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }
    public async Task UpdateSocialAccountAsync(Guid id, UpdateSocialAccountDto update)
    {
        var existing = await _dbContext.SocialAccounts
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing == null)
            return;

        if (update.AccessToken != null)
            existing.AccessToken = update.AccessToken;

        if (update.RefreshToken != null)
            existing.RefreshToken = update.RefreshToken;

        if (update.TokenExpiresAt.HasValue)
            existing.TokenExpiresAt = update.TokenExpiresAt.Value;
        
        if (update.Status != existing.Status)
            existing.Status = update.Status;
        
        existing.LastSync = update.LastSync;

        await _dbContext.SaveChangesAsync();
    }

    public async Task UploadVideoAsync(SocialVideoDbM video)
    {
        _dbContext.SocialVideos.Add(video);
        await _dbContext.SaveChangesAsync();
    }

}
