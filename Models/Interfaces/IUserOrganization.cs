namespace Models;

public interface IUserOrganization
{
    Guid Id { get; set; }
    string Role { get; set; }
    DateTime CreatedAt { get; set; }

    public IOrganization Organization { get; set; }
    public IUser User { get; set; }
}
