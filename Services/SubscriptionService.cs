using DbRepos;
using Models.DTO;

namespace Services;

public class SubscriptionService : ISubscriptionService
{
    readonly SubscriptionDbRepo _repo;
    readonly OrganizationDbRepo _orgRepo;
    readonly UserDbRepo _userRepo;
    readonly IStripeService _stripeService;

    public SubscriptionService(SubscriptionDbRepo repo, OrganizationDbRepo orgRepo, IStripeService stripeService, UserDbRepo userRepo)
    {
        _repo = repo;
        _orgRepo = orgRepo;
        _stripeService = stripeService;
        _userRepo = userRepo;
    }
    public async Task<ServiceResult<string>> CreateSubscriptionCheckoutAsync(Guid organizationId, Guid subscriptionPlanId, Guid requestUserId)
    {
        var check = await _orgRepo.GetOrganizationByIdAsync(organizationId, requestUserId);
        if (check == null)
        {
            return ServiceResult<string>.Fail(""); //todo message
        }

        var userOrganization = await _orgRepo.GetUserOrganizationAsync(organizationId, requestUserId);

        if (userOrganization == null)
        {
            return ServiceResult<string>.Fail(""); //todo message
        }
        if (userOrganization.Role != "Owner" && userOrganization.Role != "Admin")
        {
            return ServiceResult<string>.Fail(""); //todo message
        }
        var subscriptionPlan = await _repo.GetSubscriptionPlanByIdAsync(subscriptionPlanId);

        if (subscriptionPlan == null)
        {
            return ServiceResult<string>.Fail("Subscription plan not found.");
        }
        if (!subscriptionPlan.isFree)
        {
            var user = await _userRepo.GetUserAsync(requestUserId);
            if (user is null || user.Email is null) return ServiceResult<string>.Fail("User not found or user have no email");

            var checkoutUrl = await _stripeService.CreateCheckoutSessionAsync(subscriptionPlan.StripePriceId, user.Email, organizationId);

            return ServiceResult<string>.Ok("Checkout session created.", checkoutUrl);
        }
        else
        {
            var subscription = new OrganizationSubscriptionUpdate
            {
                OrganizationId = organizationId,
                SubscriptionPlanId = subscriptionPlanId,
                CancelAtPeriodEnd = false,
                CurrentPeriodStart = DateTime.UtcNow,
                CurrentPeriodEnd = null,
                Status = "active",
                StripeCustomerId = null,
                StripeSubscriptionId = null

            };
            var save = await _repo.SaveOrganizationSubscriptionAsync(subscription);
            if (!save) return ServiceResult<string>.Fail("Something went wrong saving free plan");
            return ServiceResult<string>.Ok("Free Plan successful selected");
        }
    }

    public async Task<ServiceResult<List<SubscriptionDto>>> GetSubscriptionsAsync()
    {
        var subs = await _repo.GetSubscriptionsAsync();
        if (subs == null)
        {
            return ServiceResult<List<SubscriptionDto>>.Fail("Could not retrieve subscription plans.");
        }
        return ServiceResult<List<SubscriptionDto>>.Ok("Subscriptions retrieved successfully.", subs);
    }
}