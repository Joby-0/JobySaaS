namespace Models;

public class Subscription : ISubscription
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public PlanEnum Plan { get; set; }
    public string StripeCustomerId { get; set; }
    public string StripeSubscriptionId { get; set; }
    public StatusEnum Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
