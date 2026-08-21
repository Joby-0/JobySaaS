using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;

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
    public async Task<bool> SaveOrganizationSubscriptionAsync(OrganizationSubscriptionUpdate subscription)
    {
        var existing = await _dbContext.OrganizationSubscriptions.FirstOrDefaultAsync(x => x.OrganizationId == subscription.OrganizationId); //todo SubscriptionPlanId ska ocksp var unik

        if (existing != null)
        {
            existing.SubscriptionPlanId = subscription.SubscriptionPlanId;
            existing.StripeCustomerId = subscription.StripeCustomerId;
            existing.StripeSubscriptionId = subscription.StripeSubscriptionId;
            existing.Status = subscription.Status;
            existing.CurrentPeriodStart = subscription.CurrentPeriodStart;
            existing.CurrentPeriodEnd = subscription.CurrentPeriodEnd.Value;
            existing.CancelAtPeriodEnd = subscription.CancelAtPeriodEnd;
            // existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _dbContext.OrganizationSubscriptions.Add(
                new OrganizationSubscriptionDbM
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = subscription.OrganizationId,
                    SubscriptionPlanId = subscription.SubscriptionPlanId,
                    StripeCustomerId = subscription.StripeCustomerId,
                    StripeSubscriptionId = subscription.StripeSubscriptionId,
                    Status = subscription.Status,
                    CurrentPeriodStart = subscription.CurrentPeriodStart,
                    CurrentPeriodEnd = subscription.CurrentPeriodEnd.Value,
                    CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
                    // CreatedAt = DateTime.UtcNow,
                    // UpdatedAt = DateTime.UtcNow
                });
        }

        await _dbContext.SaveChangesAsync();

        return true;
    }
}