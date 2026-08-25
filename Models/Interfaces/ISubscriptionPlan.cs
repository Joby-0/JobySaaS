namespace Models;

public interface ISubscriptionPlan
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string StripePriceId { get; set; }
    public int Price { get; set; }
    public int BillingIntervalInMonths { get; set; }
    public bool isFree { get; set; }
    public string Description { get; set; }
    //fetures
    public List<IFeature> Features { get; set; }
}


public interface IFeature
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ISubscriptionPlan SubscriptionPlan { get; set; }
}