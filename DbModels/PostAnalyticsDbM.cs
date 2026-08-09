using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace DbModels;

public class PostAnalyticsDbM : PostAnalytics
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid PostId { get; set; }

    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }

    public DateTime ReportedAt { get; set; }

    [ForeignKey(nameof(PostId))]
    public PostDbM Post { get; set; } = null!;
}
