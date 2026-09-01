using Models;
using Models.DTO;

namespace Services;

public interface IOrganizationService
{
    Task<IOrganization> CreateOrganizationAsync(CreateOrganizationRequest request, Guid ownerId,string ownerUserName, string email);
    Task<IOrganization> GetOrganizationByIdAsync(Guid organizationId, Guid requestUserId);

    Task<List<IOrganization>> GetOrganizationsForUserAsync(Guid userId);

    Task<ServiceResult<List<OrganizationMemberDTO>>> GetOrganizationMembersAsync(Guid organizationId, Guid requestUserId);
}