using Models.DTO;

namespace Services;

public interface ISocialAccountService
{
    public Task<ServiceResult<List<SocialAccountDto>>> GetConnectedAccountsAsync(Guid orgId, Guid requestUserId);
    public Task<ServiceResult<bool>> DisconnectAccountAsync(Guid orgId, Guid requestUserId, Guid accountId);
}