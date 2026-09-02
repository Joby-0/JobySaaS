namespace Models;

public interface IMedia
{
    public Guid Id { get; set; }
    public IOrganization Organization { get; set; }

    public string FileUrl { get; set; }
    public string ThumbnailUrl { get; set; }

    public string FileName { get; set; }
    public string MimeType { get; set; }
    public long FileSize { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public TimeSpan? Duration { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<ISocialVideo> SocialVideos { get; set; }
}