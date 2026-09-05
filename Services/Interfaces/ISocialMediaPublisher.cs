using DbModels;

namespace Services;

public interface ISocialMediaPublisher
{
    Models.SocialPlatform Platform { get; }

    Task<PublishResult> PublishAsync(
        SocialAccountDbM account,
        MediaDbM media,
        CancellationToken cancellationToken);
}

public sealed record PublishResult(bool Success, string? ExternalPostId = null, string? ErrorMessage = null)
{
    public static PublishResult Completed(string? externalPostId = null) => new(true, externalPostId);
    public static PublishResult Failed(string error) => new(false, null, error);
}
