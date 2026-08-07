using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbModels;

public class SocialAccountDbM
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid OrganizationId { get; set; }

    [Required]
    [StringLength(120)]
    public string Platform { get; set; }

    [Required]
    [StringLength(120)]
    public string Username { get; set; }

    [Required]
    public string AccessToken { get; set; }

    public DateTime TokenExpires { get; set; }

    [ForeignKey(nameof(OrganizationId))]
    public OrganizationDbM Organization { get; set; } = null!;
}
