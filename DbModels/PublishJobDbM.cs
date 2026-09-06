using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models;

namespace DbModels;

public class PublishJobDbM
{
    [Key]
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid MediaId { get; set; }
    public PublishJobStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    [ForeignKey(nameof(OrganizationId))]
    [JsonIgnore]
    public OrganizationDbM OrganizationDbM { get; set; } = null!;

    [ForeignKey(nameof(MediaId))]
    [JsonIgnore]
    public MediaDbM MediaDbM { get; set; } = null!;

    public ICollection<PublishJobAccountDbM> Accounts { get; set; } 
}
