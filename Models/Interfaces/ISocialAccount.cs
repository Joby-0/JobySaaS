namespace Models;

public interface ISocialAccount
{
    Guid Id { get; set; }

    SocialPlatfrom Platform { get; set; }

    string Username { get; set; }
    string CostumUrl {get; set;}
    string ProfileImageUrl {get; set;}
    ulong? Followers {get; set;}

    string AccessToken { get; set; }
    string RefreshToken { get; set; }
    DateTime LastSync {get; set;}
    SocialAccountStatus Status {get; set;}

    DateTime CreatedAt {get; set;}
    DateTime? TokenExpiresAt { get; set; }

    IOrganization Organization { get; set; }
}

public enum SocialAccountStatus
{
    Connected,
    Expired,
    Error,
    Disconnected
}
public enum SocialPlatfrom
{
    YouTube,
    TikTok,
    X,
    LinkedIn,
    Instagram,
    Facebook
}