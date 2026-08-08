namespace Configuration;

public class JwtOptions
{
    public const string Position = "JwtConfig";

    public int LifeTimeMinutes { get; set; } = 60;
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public string IssuerSigningKey { get; set; } = "SuperSecureReferenceKeyThatShouldBeChanged";
    public bool ValidateIssuer { get; set; } = true;
    public string ValidIssuer { get; set; } = "ApiReference";
    public bool ValidateAudience { get; set; } = true;
    public string ValidAudience { get; set; } = "ApiReferenceAudience";
    public bool RequireExpirationTime { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
}
