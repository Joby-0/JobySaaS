using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace DbModels;

public class SubscriptionPlanDbM : SubscriptionPlan
{
    [Key]
    public override Guid Id { get; set; }
}
