namespace Models.DTO;

public class SocialAccountDto
{
    public Guid Id { get; set; }
    public SocialPlatform Platform { get; set; }
    public string AccountName { get; set; }
    public string CostumUrl { get; set; }
    public string ProfileImageUrl { get; set; }
    public ulong? Followers { get; set; }
    public SocialAccountStatus Status { get; set; }

    public DateTime LastSync { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateSocialAccountDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    public SocialAccountStatus Status { get; set; }
    public DateTime LastSync { get; set; }
}

public class SocialAccountDetails
{
    public string AccountName {get; set;}
    public string Handle {get; set;}
    public string ProfileImageUrl {get; set;}
    public ulong? Followers {get; set;}
    public ulong? Views {get; set;}
    public ulong? VideoCount {get; set;}
    public ulong? CommentCount {get;set;}

    public string AccountBanner {get;set;}

    public SocialPlatform Platform {get; set;}
}