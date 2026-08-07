namespace Models;

public class Media : IMedia
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string FileUrl { get; set; }
    public string ThumbnailUrl { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Duration { get; set; }
    public DateTime CreatedAt { get; set; }
}
