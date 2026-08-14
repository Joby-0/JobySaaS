namespace Models.DTO;

public class CreateOrganizationRequest
{
    public string Name { get; set; }
}
public class SelectSubscriptionRequest
{
    public Guid SubscriptionId { get; set; }
}
public class OrganizationSubscriptionUpdate
{
    public Guid OrganizationId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public string StripeCustomerId { get; set; }
    public string StripeSubscriptionId { get; set; }
    public string Status { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
}