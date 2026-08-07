using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbModels;

public class MediaDbM
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid OrganizationId { get; set; }

    [Required]
    [StringLength(256)]
    public string FileUrl { get; set; }

    [StringLength(256)]
    public string ThumbnailUrl { get; set; }

    [Required]
    [StringLength(120)]
    public string Title { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [StringLength(50)]
    public string Duration { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(OrganizationId))]
    public OrganizationDbM Organization { get; set; } = null!;
}
