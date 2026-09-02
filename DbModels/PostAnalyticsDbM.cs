using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace DbModels;

public class PostAnalyticsDbM : PostAnalytics
{
    [Key]
    public override Guid Id { get; set; }

    [Required]
    public Guid SocialVideoId { get; set; }

    [ForeignKey(nameof(SocialVideoId))]
    public ISocialVideo SocialVideo { get; set; } = null!;
}
