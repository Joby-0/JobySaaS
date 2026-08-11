using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models;
namespace DbModels;

public class UserOrganizationDbM : UserOrganization
{
    [Key]
    public override Guid Id { get; set; }

    [NotMapped]
    public override IUser User { get => UserDbM; set => new NotImplementedException(); }
    [JsonIgnore]
    public Guid UserId { get; set; }

    [JsonIgnore]
    [ForeignKey("UserId")]
    public UserDbM UserDbM { get; set; }

    [NotMapped]
    public override IOrganization Organization { get => OrganizationDbM; set => new NotImplementedException(); }
    [JsonIgnore]
    public Guid OrganizationId { get; set; }
    
    [JsonIgnore]
    [ForeignKey("OrganizationId")]
    public OrganizationDbM OrganizationDbM { get; set; }
}
