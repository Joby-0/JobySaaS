using System.ComponentModel.DataAnnotations;
using DbModels;

namespace DbModels;

public class UserDbM
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(120)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(256)]
    public string Email { get; set; }

    [Required]
    public string Passwordhash { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<UserOrganizationDbM> Organizations { get; set; } = new List<UserOrganizationDbM>();
}
