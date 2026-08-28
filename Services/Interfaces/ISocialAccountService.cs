using Models.DTO;

namespace Services;

public interface ISocialAccountService
{
    public Task<ServiceResult<List<SocialAccountDto>>> GetConnectedAccountsAsync(Guid orgId, Guid requestUserId);
}