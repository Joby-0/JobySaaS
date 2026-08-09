using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace DbModels;

public class SubscriptionDbM : Subscription
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid OrganizationId { get; set; }

    public PlanEnum Plan { get; set; }

    [Required]
    [StringLength(256)]
    public string StripeCustomerId { get; set; }

    [Required]
    [StringLength(256)]
    public string StripeSubscriptionId { get; set; }

    public StatusEnum Status { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(OrganizationId))]
    public OrganizationDbM Organization { get; set; } = null!;
}
