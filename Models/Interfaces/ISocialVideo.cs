public interface ISocialVideo
{
    public Guid Id { get; set; }

    public string VideoId { get; set; }

    public VideoUploadStatus Status { get; set; }

    public int? ProcessingPercentage { get; set; }

    public string? FailureReason { get; set; }

    public DateTime CreatedAt { get; set; }
}
public enum VideoUploadStatus
{
    Pending,
    Processing,
    Succeeded,
    Completed,
    Failed
}