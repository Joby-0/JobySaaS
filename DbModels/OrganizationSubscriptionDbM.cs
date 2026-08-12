using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models;

namespace DbModels;

public class OrganizationSubscriptionDbM : OrganizationSubscription
{
    [Key]
    public override Guid Id { get; set; }

    [NotMapped]
    public override IOrganization Organization { get => OrganizationDbM; set => new NotImplementedException(); }
    [JsonIgnore]
    public Guid OrganizationId { get; set; }
    [ForeignKey("OrganizationId")]
    [JsonIgnore]
    public OrganizationDbM OrganizationDbM { get; set; }

    [NotMapped]
    public override ISubscriptionPlan SubscriptionPlan { get => SubscriptionPlanDbM; set => new NotImplementedException(); }
    [JsonIgnore]
    public Guid SubscriptionPlanId {get; set;}
    [ForeignKey("SubscriptionPlanId")]
    [JsonIgnore]
    public SubscriptionPlanDbM SubscriptionPlanDbM {get; set;}
}