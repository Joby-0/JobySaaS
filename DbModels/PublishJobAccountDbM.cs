using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models;

namespace DbModels;

public class PublishJobAccountDbM
{
    [Key]
    public Guid Id { get; set; }
    public Guid PublishJobId { get; set; }
    public Guid SocialAccountId { get; set; }
    public PublishJobAccountStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ExternalPostId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    [ForeignKey(nameof(PublishJobId))]
    [JsonIgnore]
    public PublishJobDbM PublishJob { get; set; } = null!;

    [ForeignKey(nameof(SocialAccountId))]
    [JsonIgnore]
    public SocialAccountDbM SocialAccount { get; set; } = null!;
}
