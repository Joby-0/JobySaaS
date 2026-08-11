namespace Models;

public interface ISubscriptionPlan
{
    public Guid Id { get; set; }

    public string Name {get; set;}
    public string StripePriceId {get;set;}
    public int Price {get;set;}
    public int BillingIntervalInMonths {get;set;}

    //fetures
}