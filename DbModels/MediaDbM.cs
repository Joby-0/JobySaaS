using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models;

namespace DbModels;

public class MediaDbM : Media
{
    [Key]
    public Guid Id { get; set; }

    [NotMapped]
    public override IOrganization Organization { get => OrganizationDbM; set => new NotImplementedException(); }

    [JsonIgnore]
    public Guid OrganizationId { get; set; }

    [ForeignKey("OrganizationId")]
    [JsonIgnore]
    public OrganizationDbM OrganizationDbM { get; set; }
}
