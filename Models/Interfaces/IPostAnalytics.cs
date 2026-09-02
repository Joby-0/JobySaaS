namespace Models;

public interface IPostAnalytics
{
    Guid Id { get; set; }
    ISocialVideo SocialVideo { get; set; }
    int Views { get; set; }
    int Likes { get; set; }
    int Comments { get; set; }
    int Shares { get; set; }
    DateTime ReportedAt { get; set; }
}