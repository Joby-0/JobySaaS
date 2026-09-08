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
    public async Task<ServiceResult<OrganizationDto>> CreateOrganizationAsync(CreateOrganizationRequest request, Guid ownerId, string ownerUserName, string email)
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

        return ServiceResult<OrganizationDto>.Ok("Created successfully new organization", new OrganizationDto(save));

    }

    public async Task<ServiceResult<OrganizationDto>> GetOrganizationByIdAsync(Guid organizationId, Guid requestUserId)
    {
        var result = await _repo.GetOrganizationByIdAsync(organizationId, requestUserId);

        return ServiceResult<OrganizationDto>.Ok("Successfully get organization",new OrganizationDto(result));
    }

    public async Task<ServiceResult<List<OrganizationMemberDTO>>> GetOrganizationMembersAsync(Guid organizationId, Guid requestUserId)
    {
        var userOrganization = await _repo.GetUserOrganizationAsync(organizationId, requestUserId);
        if (userOrganization == null)
        {
            return ServiceResult<List<OrganizationMemberDTO>>.Fail("You do not have access to this organization.");
        }

        var members = await _repo.GetMembers(organizationId);
        return  ServiceResult<List<OrganizationMemberDTO>>.Ok("successfully", members);
    }

    public async Task<ServiceResult<List<OrganizationDto>>> GetOrganizationsForUserAsync(Guid userId)
    {
        var result = await _repo.GetOrganizationsForUserAsync(userId);
        return  ServiceResult<List<OrganizationDto>>.Ok("successfully", result);
    }

    public async Task<ServiceResult<string>> RemoveOrganizationMemberAsync(Guid organizationId, Guid memberUserId, Guid requestUserId)
    {
        var userOrganization = await _repo.GetUserOrganizationAsync(organizationId, requestUserId);
        if (userOrganization == null)
        {
            return ServiceResult<string>.Fail("You do not have access to this organization.");
        }
        if (userOrganization.Role != "Owner" && userOrganization.Role != "Admin")
        {
            return ServiceResult<string>.Fail("You do not have access to this organization.");
        }

        await _repo.RemoveOrganizationMemberAsync(organizationId, memberUserId);

        return  ServiceResult<string>.Ok("Member removed successfully.");

    }
}