namespace Services;

using Configuration.Options;
using DbRepos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.DTO;
using Stripe;
using Stripe.Checkout;

public class StripeService : IStripeService
{
    private readonly StripeOptions _options;
    private readonly ILogger<IStripeService> _logger;

    private readonly SubscriptionDbRepo _subscriptionDbRepo;

    public StripeService(IOptions<StripeOptions> options, ILogger<IStripeService> logger, SubscriptionDbRepo subscriptionDbRepo)
    {
        _logger = logger;
        _options = options.Value;
        _subscriptionDbRepo = subscriptionDbRepo;

        StripeConfiguration.ApiKey = _options.SecretKey;
    }

    public async Task<string> CreateCheckoutSessionAsync(string stripePriceId, string customerEmail, Guid organizationId)
    {
        var options = new SessionCreateOptions
        {
            Mode = "subscription",

            CustomerEmail = customerEmail,

            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = stripePriceId,
                    Quantity = 1
                }
            },

            SuccessUrl = "https://localhost:7249/payment/success?session_id={CHECKOUT_SESSION_ID}",

            CancelUrl = "https://localhost:7249/payment/cancel",

            Metadata = new Dictionary<string, string>
            {
                ["OrganizationId"] = organizationId.ToString()
            }
        };

        var service = new SessionService();

        var session = await service.CreateAsync(options);

        return session.Url;
    }

    public Task<ServiceResult<string>> HandleCheckoutCompletedAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<string>> HandleSubscriptionDeletedAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<string>> HandleSubscriptionUpdatedAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> HandleWebhookAsync(string json, string signature)
    {
        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature, _options.WebhookSecret);
        }
        catch (StripeException ex)
        {
            _logger.LogWarning(ex, "Invalid Stripe webhook signature.");

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to construct Stripe webhook event.");

            return false;
        }

        _logger.LogInformation("Received Stripe event: {EventType}", stripeEvent.Type);

        switch (stripeEvent.Type)
        {
            case EventTypes.CheckoutSessionCompleted:
                {
                    var session = stripeEvent.Data.Object as Session;

                    if (session == null)
                    {
                        return false;
                    }
                    if (string.IsNullOrEmpty(session.SubscriptionId))
                    {
                        _logger.LogError("Checkout session {SessionId} does not contain a subscription ID.", session.Id);

                        return false;
                    }

                    var organizationIdString = session.Metadata.GetValueOrDefault("OrganizationId");

                    if (!Guid.TryParse(organizationIdString, out var organizationId))
                    {
                        _logger.LogError("Stripe checkout session {SessionId} has an invalid OrganizationId.", session.Id);

                        return false;
                    }

                    var stripeCustomerId = session.CustomerId;

                    var stripeSubscriptionId = session.SubscriptionId;

                    _logger.LogInformation("Checkout completed for organization {OrganizationId}. Stripe subscription: {SubscriptionId}", organizationId, stripeSubscriptionId);

                    // Database handling comes next.
                    var subscription = new OrganizationSubscriptionUpdate
                    {
                        StripeCustomerId = stripeCustomerId,
                        StripeSubscriptionId = stripeSubscriptionId,
                        OrganizationId = organizationId,

                    };
                    await _subscriptionDbRepo.SaveOrganizationSubscriptionAsync(subscription);

                    break;
                }

            default:
                {

                    _logger.LogInformation("Unhandled Stripe event: {EventType}", stripeEvent.Type);

                    break;
                }
        }

        return true;
    }
}