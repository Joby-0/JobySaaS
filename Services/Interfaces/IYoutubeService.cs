using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Http;
using Models;
using System.Threading.Tasks;
namespace Services;

public interface IYoutubeService
{
    // Task<string> UploadAsync(string filePath, string title, string description, string[] tags);
    Task<ServiceResult> Callback(string code);
    Task<ServiceResult> Connect();

    Task<ServiceResult> UploadVideoAsync(IFormFile video, string title, string description, string categoryId, ISocialAccount account);

    Task<ServiceResult> RefreshTokenAsync(ISocialAccount socialAccount);
    Task<ServiceResult> GetAccessTokenAsync(ISocialAccount account);
}