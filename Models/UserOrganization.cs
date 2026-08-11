namespace Models;

public class UserOrganization : IUserOrganization
{
    public virtual Guid Id { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual IOrganization Organization { get; set; }
    public virtual IUser User { get; set; }
}
