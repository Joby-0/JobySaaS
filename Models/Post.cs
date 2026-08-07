namespace Models;

public class Post : IPost
{
    public Guid Id { get; set; }
    public Guid MediaId { get; set; }
    public Guid SocialAccountId { get; set; }
    public string Status { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime PublishedAt { get; set; }
    public Guid PlatformPostId { get; set; }
}
