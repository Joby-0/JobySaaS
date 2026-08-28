namespace Models;

public interface ISocialAccount
{
    Guid Id { get; set; }

    Guid OrganizationId { get; set; }

    string Platform { get; set; }

    string Username { get; set; }

    string AccessToken { get; set; }
    string RefreshToken { get; set; }

    DateTime CreatedAt {get; set;}
    DateTime? TokenExpiresAt { get; set; }

    IOrganization Organization { get; set; }
}