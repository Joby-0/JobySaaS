using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbModels;

public class PostDbM
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid MediaId { get; set; }

    [Required]
    public Guid SocialAccountId { get; set; }

    [Required]
    [StringLength(120)]
    public string Status { get; set; }

    public DateTime ScheduledAt { get; set; }
    public DateTime PublishedAt { get; set; }

    public Guid PlatformPostId { get; set; }

    [ForeignKey(nameof(MediaId))]
    public MediaDbM Media { get; set; } = null!;

    [ForeignKey(nameof(SocialAccountId))]
    public SocialAccountDbM SocialAccount { get; set; } = null!;
}
