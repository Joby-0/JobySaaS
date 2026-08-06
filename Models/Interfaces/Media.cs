namespace Models;

public interface IMedia
{
    Guid Id { get; set; }
    Guid OrganizationId { get; set; }
    string FileUrl { get; set; }
    string ThumbnailUrl { get; set; }
    string Title { get; set; }
    string Description { get; set; }
    string Duration { get; set; }
    DateTime CreatedAt { get; set; }
}