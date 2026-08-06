namespace Models;

public interface IUserOrganization
{
    Guid Id { get; set; }
    Guid UserId { get; set; }
    Guid OrganizationId { get; set; }
    string Role { get; set; }
    DateTime CreatedAt { get; set; }
}
