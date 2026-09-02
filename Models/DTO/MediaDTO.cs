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