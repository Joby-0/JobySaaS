using DbRepos;

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
        }else if(subscriptionPlan.StripePriceId == null)
        {
            return ServiceResult<string>.Ok("Free Plan selected");
        }
        
        var user = await _userRepo.GetUserAsync(requestUserId);

        var checkoutUrl = await _stripeService.CreateCheckoutSessionAsync(subscriptionPlan.StripePriceId, user.Email, organizationId);

        return ServiceResult<string>.Ok("Checkout session created.", checkoutUrl);
    }
}