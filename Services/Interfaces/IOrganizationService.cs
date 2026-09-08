using Models;
using Models.DTO;

namespace Services;

public interface IOrganizationService
{
    Task<ServiceResult<OrganizationDto>> CreateOrganizationAsync(CreateOrganizationRequest request, Guid ownerId,string ownerUserName, string email);
    Task<ServiceResult<OrganizationDto>> GetOrganizationByIdAsync(Guid organizationId, Guid requestUserId);

    Task<ServiceResult<List<OrganizationDto>>> GetOrganizationsForUserAsync(Guid userId);

    Task<ServiceResult<List<OrganizationMemberDTO>>> GetOrganizationMembersAsync(Guid organizationId, Guid requestUserId);
    Task<ServiceResult<string>> RemoveOrganizationMemberAsync(Guid organizationId, Guid memberUserId, Guid requestUserId);
}