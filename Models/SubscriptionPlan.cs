namespace Models;

public class SubscriptionPlan : ISubscriptionPlan
{
    public virtual Guid Id { get; set; }
    public string Name { get; set; }
    public string StripePriceId { get; set; }
    public int Price { get; set; }
    public int BillingIntervalInMonths { get; set; }
}
