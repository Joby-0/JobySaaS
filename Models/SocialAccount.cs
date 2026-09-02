namespace Models;

public class SocialAccount : ISocialAccount
{
    public virtual Guid Id { get; set; }
    public SocialAccountPlatfrom Platform { get; set; }
    public string Username { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    public virtual IOrganization Organization { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string CostumUrl { get; set; }
    public string ProfileImageUrl { get; set; }
    public DateTime LastSync { get; set; }
    public SocialAccountStatus Status { get; set; }
    public ulong? Followers { get; set; }
}
