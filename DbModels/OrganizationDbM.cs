using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models;

namespace DbModels;

public class OrganizationDbM : Organization
{
    [Key]
    public override Guid Id { get; set; }

    [NotMapped]
    public override IOrganizationSubscription? OrganizationSubscription { get => OrganizationSubscriptionDbM; set => new NotImplementedException(); }
    [JsonIgnore]
    public Guid? OrganizationSubscriptionId { get; set; }
    [ForeignKey("OrganizationSubscriptionId")]
    [JsonIgnore]
    public OrganizationSubscriptionDbM? OrganizationSubscriptionDbM { get; set; }

    
    [NotMapped]
    public override List<IUserOrganization> Users { get => UserOrganizationDbMs?.ToList<IUserOrganization>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public ICollection<UserOrganizationDbM> UserOrganizationDbMs { get; set; }

    [NotMapped]
    public override List<ISocialAccount> SocialAccounts { get => SocialAccountDbMs?.ToList<ISocialAccount>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public ICollection<SocialAccountDbM> SocialAccountDbMs { get; set; } = new List<SocialAccountDbM>();

    [NotMapped]
     public override List<IPost> Posts { get => PostsDbMs?.ToList<IPost>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public ICollection<PostDbM> PostsDbMs { get; set; } = new List<PostDbM>();

    [NotMapped]
    public override List<IMedia> Media { get => MediaDbMs?.ToList<IMedia>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public ICollection<MediaDbM> MediaDbMs { get; set; } = new List<MediaDbM>();
}
