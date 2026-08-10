using DbModels;
using DbRepos;
using Models;
using Models.DTO;

namespace Services;

public class OrganizationService : IOrganizationService
{
    readonly OrganizationDbRepo _repo;
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
}