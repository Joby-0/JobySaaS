using Models;

namespace Models.DTO;

public sealed class PublishJobDto
{
    public Guid Id { get; set; }
    public PublishJobStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<PublishJobAccountDto> Accounts { get; set; } = new();
}

public sealed class PublishJobAccountDto
{
    public Guid SocialAccountId { get; set; }
    public SocialPlatform Platform { get; set; }
    public PublishJobAccountStatus Status { get; set; }
    public string? ExternalPostId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
