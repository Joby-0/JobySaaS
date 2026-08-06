namespace Models;

public interface ISubscription
{
    Guid Id { get; set; }
    Guid OrganizationId { get; set; }
    PlanEnum Plan { get; set; }
    string StripeCustomerId { get; set; } //todo ändra till den tjänsten som används
    string StripeSubscriptionId { get; set; } //todo ändra till den tjänsten som används
    StatusEnum Status { get; set; }
    DateTime CreatedAt { get; set; }
}