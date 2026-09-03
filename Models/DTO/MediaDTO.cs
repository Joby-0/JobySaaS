namespace Models.DTO;

public class MediaListDTO
{
    public Guid Id { get; set; }
    public string ThumbnailUrl { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTime CreatedAt { get; set; }

    public int SocialAccountCount { get; set; }
    public List<SocialPlatform> SocialPlatforms { get; set; }
}

public class MediaDetailsDTO
{
    public Guid Id { get; set; }
    public string ThumbnailUrl { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<SocialAccountDto> SocialAccounts { get; set; } //maybe u want less info here, like only the platform and the account name, not the whole social account dto
    public List<ISocialVideo> SocialVideos { get; set; } //maybe u want less info here, do a dto for this,
}

public class CreateMediaDTO
{
    public string FileUrl { get; set; }
    public string ThumbnailUrl { get; set; }
    public string FileName { get; set; }
    public string MimeType { get; set; }
    public long FileSize { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public TimeSpan? Duration { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid OrganizationId { get; set; }
}