using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;

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

    public async Task<SocialAccountDbM?> GetByIdAsync(Guid id)
    {
        return await _dbContext.SocialAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<SocialAccountDbM> AddAsync(SocialAccountDbM account)
    {
        _dbContext.SocialAccounts.Add(account);
        await _dbContext.SaveChangesAsync();
        return account;
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
}