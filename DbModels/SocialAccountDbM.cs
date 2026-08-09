using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models;

namespace DbModels;

public class SocialAccountDbM : SocialAccount
{
    [Key]
    public override Guid Id { get; set; }

    [Required]
    public override Guid OrganizationId { get; set; }

    [ForeignKey(nameof(OrganizationId))]
    [JsonIgnore]
    public OrganizationDbM OrganizationDbM { get; set; } = null!;

    [NotMapped]
    public override IOrganization Organization { get => OrganizationDbM ; set => new NotImplementedException(); }
}
