using Models;
using Models.DTO;

namespace Services;

public interface IOrganizationService
{
    Task<IOrganization> CreateOrganizationAsync(CreateOrganizationRequest request, Guid ownerId);

    Task<IOrganization> GetOrganizationByIdAsync(Guid organizationId, Guid requestUserId);
}