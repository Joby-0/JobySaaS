using DbModels;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Http;
using Models;
using Models.DTO;
using System.Threading.Tasks;
namespace Services;

public interface IYoutubeService
{
    Task<ServiceResult<Guid>> Callback(string code, string state);
    Task<ServiceResult<string>> Connect(Guid organizationId);

    Task<ServiceResult<string>> UploadVideoAsync(Guid mediaId, string title, string description, string categoryId, Guid accountId, Guid requestUserId);
    Task<ServiceResult<SocialAccountDetails>> GetAccountDetailsAsync(SocialAccountDbM account);

    Task<ServiceResult<List<DailyMetricDto>>> GetAccountPerformanceAsync(Guid accountId, DateOnly startDate, DateOnly endDate, Guid requestUserId, CancellationToken ct);

    Task<ServiceResult<string>> RefreshTokenAsync(ISocialAccount socialAccount);
    Task<ServiceResult<string>> GetAccessTokenAsync(Guid userId);
}
