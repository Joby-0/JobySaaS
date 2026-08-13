namespace Services;

public interface ISubscriptionService
{
    Task<ServiceResult<string>> CreateSubscriptionCheckoutAsync(Guid organizationId, Guid subscriptionPlanId, Guid requestUserId);
}