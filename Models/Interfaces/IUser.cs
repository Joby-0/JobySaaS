namespace Models;

public interface IUser
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    // public string ProfileImage { get; set; }
    // public string FirstName { get; set; }
    // public string LastName { get; set; }
    public string Email { get; set; }

    // public string PasswordHash { get; set; }
    // public UserRolesEnum Role {get; set;}
    public DateTime CreatedAt { get; set; }
    // public DateTime UpdatedAt { get; set; }
    // public bool IsDeleted { get; set; }

    List<IUserOrganization> Organizations { get; set; }
}