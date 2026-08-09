using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace DbModels;

public class OrganizationDbM : Organization
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required]
    public Guid OwnerId { get; set; }

    public SubscriptionDbM Subscription { get; set; } = null!;
    public ICollection<UserOrganizationDbM> Users { get; set; } = new List<UserOrganizationDbM>();
    public ICollection<SocialAccountDbM> SocialAccounts { get; set; } = new List<SocialAccountDbM>();
    public ICollection<PostDbM> Posts { get; set; } = new List<PostDbM>();
    public ICollection<MediaDbM> Media { get; set; } = new List<MediaDbM>();
}
