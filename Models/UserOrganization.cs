namespace Models;

public class UserOrganization : IUserOrganization
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
