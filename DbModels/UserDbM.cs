using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models;

namespace DbModels;

public class UserDbM : User
{
    [Key]
    public override Guid Id { get; set; }

    [NotMapped]
    public override List<IUserOrganization> Organizations { get => UserOrganizationDbms?.ToList<IUserOrganization>(); set => new NotImplementedException(); }

    [JsonIgnore]
    public List<UserOrganizationDbM> UserOrganizationDbms { get; set; }
}
