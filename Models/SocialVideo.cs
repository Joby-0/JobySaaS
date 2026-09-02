namespace Models;
public class SocialVideo : ISocialVideo
{
    public virtual Guid Id { get; set; }

    public string VideoId { get; set; }

    public VideoUploadStatus Status { get; set; }

    public int? ProcessingPercentage { get; set; }

    public string? FailureReason { get; set; }

    public DateTime CreatedAt { get; set; }
    public SocialPlatform Platform { get; set; }
    public virtual IMedia Media { get; set; }
}