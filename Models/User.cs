namespace Models;

public class User : IUser
{
    public virtual Guid Id { get; set; }

    public string UserName { get; set; }
    public string ProfileImage { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public string PasswordHash { get; set; }
    public UserRolesEnum Role {get; set;}
    public DateTime Created_at { get; set; }
    public DateTime Updated_at { get; set; }
    public bool IsDeleted { get; set; }
    public virtual List<IUserOrganization> Organizations { get; set; } = new();
}
