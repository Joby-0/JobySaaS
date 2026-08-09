using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;
namespace DbModels;

public class UserOrganizationDbM : UserOrganization
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid OrganizationId { get; set; }

    [Required]
    [StringLength(120)]
    public string Role { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserDbM User { get; set; } = null!;

    [ForeignKey(nameof(OrganizationId))]
    public OrganizationDbM Organization { get; set; } = null!;
}
