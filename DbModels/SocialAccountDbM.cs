using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace DbModels;

public class SocialAccountDbM : SocialAccount
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
    [Required]
    public string RefreshToken { get; set; }

    public DateTime TokenExpires { get; set; }

    [ForeignKey(nameof(OrganizationId))]
    public OrganizationDbM Organization { get; set; } = null!;
}
