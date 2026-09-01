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
        await _userRepo.EnsureUserExistsAsync(ownerId, ownerUserName, email);

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

    public async Task<ServiceResult<List<OrganizationMemberDTO>>> GetOrganizationMembersAsync(Guid organizationId, Guid requestUserId)
    {
        var userOrganization = await _repo.GetUserOrganizationAsync(organizationId, requestUserId);
        if (userOrganization == null)
        {
            return new ServiceResult<List<OrganizationMemberDTO>>
            {
                Success = false,
                Message = "You do not have access to this organization."
            };
        }

        var members = await _repo.GetMembers(organizationId);
        return new ServiceResult<List<OrganizationMemberDTO>>
        {
            Success = true,
            Data = members
        };
    }

    public Task<List<IOrganization>> GetOrganizationsForUserAsync(Guid userId) => _repo.GetOrganizationsForUserAsync(userId);

    public async Task<ServiceResult<string>> RemoveOrganizationMemberAsync(Guid organizationId, Guid memberUserId, Guid requestUserId)
    {
        var userOrganization = await _repo.GetUserOrganizationAsync(organizationId, requestUserId);
        if (userOrganization == null)
        {
            return new ServiceResult<string>
            {
                Success = false,
                Message = "You do not have access to this organization."
            };
        }
        if (userOrganization.Role != "Owner" && userOrganization.Role != "Admin")
        {
            return new ServiceResult<string>
            {
                Success = false,
                Message = "You do not have access to this organization."
            };
        }

        await _repo.RemoveOrganizationMemberAsync(organizationId, memberUserId);

        return new ServiceResult<string>
        {
            Success = true,
            Data = "Member removed successfully."
        };
    }
}