namespace Models;

public class SocialAccount : ISocialAccount
{
    public virtual Guid Id { get; set; }
    public virtual Guid OrganizationId { get; set; }
    public string Platform { get; set; }
    public string Username { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime TokenExpires { get; set; }
    public virtual IOrganization Organization { get; set; } = null!;
}
