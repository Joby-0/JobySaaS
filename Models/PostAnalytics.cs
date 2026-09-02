namespace Models;

public class PostAnalytics : IPostAnalytics
{
    public virtual Guid Id { get; set; }
    public Guid SocialVideoId { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public DateTime ReportedAt { get; set; }
}
