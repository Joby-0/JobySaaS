namespace Models;

public class SubscriptionPlan : ISubscriptionPlan
{
    public virtual Guid Id { get; set; }
    public string Name { get; set; }
    public string StripePriceId { get; set; }
    public int Price { get; set; }
    public int BillingIntervalInMonths { get; set; }
    public bool isFree { get; set; }
    public string Description { get; set; }
    public virtual List<IFeature> Features { get; set; }
}

public class Feature : IFeature
{
    public virtual Guid Id { get; set; }
    public string Name { get; set; }
    public virtual ISubscriptionPlan SubscriptionPlan { get; set; }
}