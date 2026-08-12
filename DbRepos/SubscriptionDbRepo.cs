using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DbRepos;

public class SubscriptionDbRepo
{
    private readonly MainDbContext _dbContext;

    public SubscriptionDbRepo(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ISubscriptionPlan> GetSubscriptionPlanByIdAsync(Guid subscriptionId)
    {
        var sub = await _dbContext.SubscriptionPlans.Where(x => x.Id == subscriptionId).FirstOrDefaultAsync();

        return sub;
    }
    public async Task<bool> SaveOrganizationSubscriptionAsync(Guid organizationId, Guid subscriptionPlanId)
    {
        var existing = await _dbContext.OrganizationSubscriptions.FirstOrDefaultAsync(x => x.OrganizationId == organizationId);

        if (existing != null)
        {
            existing.SubscriptionPlanId = subscriptionPlanId;
        }
        else
        {
            _dbContext.OrganizationSubscriptions.Add(
                new OrganizationSubscriptionDbM
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    SubscriptionPlanId = subscriptionPlanId
                });
        }

        await _dbContext.SaveChangesAsync();

        return true;
    }
}