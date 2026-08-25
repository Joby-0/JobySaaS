using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models;

namespace DbModels;

public class SubscriptionPlanDbM : SubscriptionPlan
{
    [Key]
    public override Guid Id { get; set; }

    [NotMapped]
    public override List<IFeature> Features { get => FeatureDbMs?.ToList<IFeature>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public List<FeatureDbM> FeatureDbMs { get; set; }
}

public class FeatureDbM : Feature
{
    public override Guid Id { get; set; }

    [NotMapped]
    public override ISubscriptionPlan SubscriptionPlan { get => SubscriptionPlanDbM; set => new NotImplementedException(); }
    [JsonIgnore]
    public Guid SubscriptionPlanId { get; set; }
    
    [Required]
    [ForeignKey("SubscriptionPlanId")]

    [JsonIgnore]
    public SubscriptionPlanDbM SubscriptionPlanDbM { get; set; }
}
