namespace Models;

public class SocialAccount : ISocialAccount
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Platform { get; set; }
    public string Username { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime TokenExpires { get; set; }
    public IOrganization Organization { get; set; } = null!;
}
