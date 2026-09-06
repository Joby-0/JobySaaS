using DbModels;
using Models;

namespace Services;

public sealed class YoutubePublisher : ISocialMediaPublisher
{
    private readonly IYoutubeService _youtube;

    public YoutubePublisher(IYoutubeService youtube) => _youtube = youtube;

    public SocialPlatform Platform => SocialPlatform.YouTube;

    public async Task<PublishResult> PublishAsync(SocialAccountDbM account, MediaDbM media, CancellationToken cancellationToken)
    {
        var result = await _youtube.UploadVideoAsync(
            media.Id, media.Title, media.Description, "22", account.Id, account.OrganizationDbM.OwnerId);

        return result.Success
            ? PublishResult.Completed(result.Data)
            : PublishResult.Failed(result.Error ?? result.Message ?? "YouTube publishing failed.");
    }
}
