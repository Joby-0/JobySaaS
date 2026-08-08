namespace Services;

public interface IYoutubeService
{
    // Task<string> UploadAsync(string filePath, string title, string description, string[] tags);
    Task<string> Callback(string code);
    Task<string> Connect();
}