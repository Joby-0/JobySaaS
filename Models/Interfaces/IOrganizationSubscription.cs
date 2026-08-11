namespace Models;

public interface IOrganizationSubscription
{
    public Guid Id {get; set;}
    public Guid OrganizationId {get; set;}
    public Guid PlanId {get;set;}
    public StatusEnum Status {get; set;}
    public string StripeSubscriptionId {get; set;}
    public DateTime CurrentPeriodStart {get; set;}
    public DateTime CurrentPeriodEnd {get; set;}
    public DateTime CancelAtPeriodEnd {get; set;}
}