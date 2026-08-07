namespace Models;

public class PostAnalytics : IPostAnalytics
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public DateTime ReportedAt { get; set; }
}
