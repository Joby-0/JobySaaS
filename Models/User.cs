namespace Models;

public class User : IUser
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }
    public string Passwordhash { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<IUserOrganization> Organizations { get; set; } = new();
}
