using System.Security.Cryptography;
using DbModels;
using DbRepos;
using Models;
using Models.DTO;

namespace Services;

public class OrganizationService : IOrganizationService
{
    readonly OrganizationDbRepo _repo;
    readonly UserDbRepo _userRepo;
    public OrganizationService(OrganizationDbRepo organizationDbRepo, UserDbRepo userDbRepo)
    {
        _repo = organizationDbRepo;
        _userRepo = userDbRepo;
    }
    public async Task<IOrganization> CreateOrganizationAsync(CreateOrganizationRequest request, Guid ownerId, string ownerUserName, string email)
    {
        await _userRepo.EnsureUserExistsAsync(ownerId, ownerUserName,email);

        var organization = new OrganizationDbM
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow
        };
        var save = await _repo.CreateOrganizationAsync(organization);


        return save;
    }

    public Task<IOrganization> GetOrganizationByIdAsync(Guid organizationId, Guid requestUserId) => _repo.GetOrganizationByIdAsync(organizationId, requestUserId);

}