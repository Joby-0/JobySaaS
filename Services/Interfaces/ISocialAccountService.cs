using Models.DTO;

namespace Services;

public interface ISocialAccountService
{
    public Task<ServiceResult<List<SocialAccountDto>>> GetConnectedAccountsAsync(Guid orgId, Guid requestUserId);
    public Task<ServiceResult<bool>> DisconnectAccountAsync(Guid orgId, Guid requestUserId, Guid accountId);
    public Task<ServiceResult<SocialAccountDetails>> GetAccountDetailsAsync(Guid orgId, Guid requestUserId, Guid accountId);
    public Task<ServiceResult<List<DailyMetricDto>>> GetAccountPerformanceAsync(Guid orgId, Guid requestUserId, Guid accountId, DateOnly startDate, DateOnly endDate, CancellationToken ct);
}