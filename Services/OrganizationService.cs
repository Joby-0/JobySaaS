using DbModels;
using DbRepos;
using Models;
using Models.DTO;

namespace Services;

public class OrganizationService : IOrganizationService
{
    readonly OrganizationDbRepo _repo;
    readonly SubscriptionDbRepo _subRepo;
    public OrganizationService(OrganizationDbRepo organizationDbRepo)
    {
        _repo = organizationDbRepo;
    }
    public async Task<IOrganization> CreateOrganizationAsync(CreateOrganizationRequest request, Guid ownerId)
    {

        var organization = new OrganizationDbM
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow
        };
        var save = await _repo.CreateOrganizationAsync(organization);


        return organization;
    }

    public Task<IOrganization> GetOrganizationByIdAsync(Guid organizationId, Guid requestUserId) => _repo.GetOrganizationByIdAsync(organizationId, requestUserId);

    public async Task<ServiceResult> UpdateSubscriptionAsync(Guid organizationId, Guid subscriptionPlanId, Guid requestUserId)
    {
        var check = await _repo.GetOrganizationByIdAsync(organizationId, requestUserId);
        if (check == null)
        {
            return new ServiceResult
            {
                Success = false,
                Message = "Error, organization was not found or you are not authizried"
            };
        }
        //check users role in org
        var userOrganization = await _repo.GetUserOrganizationAsync(organizationId, requestUserId);

        if (userOrganization == null)
        {
            return new ServiceResult
            {
                Success = false,
                Message = "You are not a member of this organization."
            };
        }
        if (userOrganization.Role != "Owner" && userOrganization.Role != "Admin")
        {
            return new ServiceResult
            {
                Success = false,
                Message = "You are not authorized to change the subscription."
            };
        }
        var subscriptionPlan = await _subRepo.GetSubscriptionPlanByIdAsync(subscriptionPlanId);

        if (subscriptionPlan == null)
        {
            return new ServiceResult
            {
                Success = false,
                Message = "Subscription plan not found."
            };
        }
        // 4. Stripe
        // Create/change Stripe subscription here

        var saveSubscription = await _subRepo.SaveOrganizationSubscriptionAsync(organizationId, subscriptionPlan.Id);
        if (!saveSubscription)
        {
            return new ServiceResult
            {
                Success = false,
                Message = "Something went wrong when saving the plan"
            };
        }

        return new ServiceResult
        {
            Success = true,
            Message = "Updating the subscription was a success"
        };
    }
}