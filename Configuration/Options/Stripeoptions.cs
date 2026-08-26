namespace Configuration.Options;
public class StripeOptions
{
    public const string Position = "Stripe";
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string FrontendBaseUrl { get; set; }
}