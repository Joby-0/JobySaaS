using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Http;
using Models;
using System.Threading.Tasks;
namespace Services;

public interface IYoutubeService
{
    // Task<string> UploadAsync(string filePath, string title, string description, string[] tags);
    Task<ServiceResult<Guid>> Callback(string code, string state);
    Task<ServiceResult<string>> Connect(Guid organizationId);

    Task<ServiceResult<string>> UploadVideoAsync(Guid mediaId, string title, string description, string categoryId, Guid accountId, Guid requestUserId);

    Task<ServiceResult<string>> RefreshTokenAsync(ISocialAccount socialAccount);
    Task<ServiceResult<string>> GetAccessTokenAsync(Guid userId);
}
