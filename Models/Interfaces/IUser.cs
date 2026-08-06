namespace Models;

public interface IUser
{
    Guid Id { get; set; }
    string FirstName { get; set; }
    string Email { get; set; }
    string Passwordhash { get; set; }
    DateTime CreatedAt { get; set; }

    List<IUserOrganization> Organizations { get; set; }
}