using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models;

namespace DbModels;

public class PostAnalyticsDbM : PostAnalytics
{
    [Key]
    public override Guid Id { get; set; }

    [NotMapped]
    public override ISocialVideo SocialVideo { get => SocialVideoDbM; set => new NotImplementedException(); }

    [JsonIgnore]
    public Guid SocialVideoId { get; set; }
    [ForeignKey("SocialVideoId")]
    [JsonIgnore]
    public SocialVideoDbM SocialVideoDbM { get; set; }
}
