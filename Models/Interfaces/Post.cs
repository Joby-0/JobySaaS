namespace Models;

public interface IPost
{
    Guid Id { get; set; }
    Guid MediaId { get; set; }
    Guid SocialAccountId { get; set; }
    string Status { get; set; }
    DateTime ScheduledAt { get; set; }
    DateTime PublishedAt { get; set; }
    Guid PlatformPostId { get; set; }

}