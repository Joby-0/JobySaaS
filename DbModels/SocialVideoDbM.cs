using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models;

namespace DbModels;

public class SocialVideoDbM : SocialVideo
{
    public override Guid Id { get; set; }

    [NotMapped]
    
    public override IMedia Media { get => MediaDbM; set => throw new NotImplementedException(); }

    [JsonIgnore]
    public Guid MediaId { get; set; }
    [JsonIgnore]
    [ForeignKey("MediaId")]
    public MediaDbM MediaDbM { get; set; }

}