using Google.Apis.Auth.OAuth2.Responses;
using System.Threading.Tasks;
namespace Services;

public interface IYoutubeService
{
    // Task<string> UploadAsync(string filePath, string title, string description, string[] tags);
    Task<ServiceResult> Callback(string code);
    Task<ServiceResult> Connect();
}