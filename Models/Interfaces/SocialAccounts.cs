namespace Models;

public interface ISocialAccount
{
    Guid Id { get; set; }

    Guid OrganizationId { get; set; }

    string Platform { get; set; }

    string Username { get; set; }

    string AccessToken { get; set; }

    DateTime TokenExpires { get; set; }

    IOrganization Organization { get; set; }
}