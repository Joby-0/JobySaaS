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

    Task<ServiceResult<string>> UploadVideoAsync(IFormFile video, string title, string description, string categoryId, ISocialAccount account);

    Task<ServiceResult<string>> RefreshTokenAsync(ISocialAccount socialAccount);
    Task<ServiceResult<string>> GetAccessTokenAsync(ISocialAccount account);
}