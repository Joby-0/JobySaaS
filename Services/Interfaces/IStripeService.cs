namespace Services;

public interface IStripeService
{
    Task<string> CreateCheckoutSessionAsync(string stripePriceId, string customerEmail, Guid organizationId);
    Task<bool> HandleWebhookAsync(string json, string signature);
    Task<ServiceResult<string>> HandleCheckoutCompletedAsync();
    Task<ServiceResult<string>> HandleSubscriptionUpdatedAsync();
    Task<ServiceResult<string>> HandleSubscriptionDeletedAsync();
    
}