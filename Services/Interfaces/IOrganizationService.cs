using Models;
using Models.DTO;

namespace Services;

public interface IOrganizationService
{
    Task<IOrganization> CreateOrganizationAsync(CreateOrganizationRequest request, Guid ownerId,string ownerUserName, string email);
    Task<IOrganization> GetOrganizationByIdAsync(Guid organizationId, Guid requestUserId);
}