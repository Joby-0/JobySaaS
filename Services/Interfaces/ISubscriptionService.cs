using Models.DTO;

namespace Services;

public interface ISubscriptionService
{
    Task<ServiceResult<string>> CreateSubscriptionCheckoutAsync(Guid organizationId, Guid subscriptionPlanId, Guid requestUserId);
    Task<ServiceResult<List<SubscriptionDto>>> GetSubscriptionsAsync();
    Task<ServiceResult<OrganizationSubscriptionStatusDto>> GetSubscriptionStatusAsync(Guid organizationId, Guid requestUserId);

}