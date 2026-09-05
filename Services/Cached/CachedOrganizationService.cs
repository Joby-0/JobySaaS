
using Microsoft.Extensions.Caching.Memory;
using Models;
using Models.DTO;

namespace Services;

public class CachedOrganizationService : IOrganizationService
{
    private readonly IOrganizationService _service;
    private readonly IMemoryCache _cache;

    public CachedOrganizationService(
        IOrganizationService organizationService,
        IMemoryCache cache)
    {
        _service = organizationService;
        _cache = cache;
    }

    public Task<IOrganization> CreateOrganizationAsync(CreateOrganizationRequest request, Guid ownerId, string ownerUserName, string email) => _service.CreateOrganizationAsync(request, ownerId, ownerUserName, email);

    public Task<IOrganization> GetOrganizationByIdAsync(Guid organizationId, Guid requestUserId) => _service.GetOrganizationByIdAsync(organizationId, requestUserId);

    public Task<ServiceResult<List<OrganizationMemberDTO>>> GetOrganizationMembersAsync(Guid organizationId, Guid requestUserId) => _service.GetOrganizationMembersAsync(organizationId, requestUserId);

    public async Task<List<OrganizationDto>> GetOrganizationsForUserAsync(Guid userId)
    {
        var key = $"organizations:{userId}";

        if (_cache.TryGetValue(key, out List<OrganizationDto>? organizations))
            return organizations!;

        organizations = await _service.GetOrganizationsForUserAsync(userId);

        _cache.Set(key, organizations, TimeSpan.FromMinutes(5));

        return organizations;
    }

    public Task<ServiceResult<string>> RemoveOrganizationMemberAsync(Guid organizationId, Guid memberUserId, Guid requestUserId) => _service.RemoveOrganizationMemberAsync(organizationId, memberUserId, requestUserId);
}