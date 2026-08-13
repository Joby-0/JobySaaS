namespace Models;

public class OrganizationSubscription : IOrganizationSubscription
{
    public virtual Guid Id { get; set; }
    public virtual IOrganization Organization { get; set; }
    public virtual ISubscriptionPlan SubscriptionPlan { get; set; }
    public string Status { get; set; }
    public string StripeSubscriptionId { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public string StripeCustomerId { get; set; }
}