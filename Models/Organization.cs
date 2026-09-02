namespace Models;

public class Organization : IOrganization
{
    public virtual Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid OwnerId { get; set; }
    public virtual IOrganizationSubscription? OrganizationSubscription { get; set; }
    public virtual List<IUserOrganization> Users { get; set; }
    public virtual List<ISocialAccount> SocialAccounts { get; set; }
    public virtual List<ISocialVideo> SocialVideos { get; set; }
    public virtual List<IMedia> Media { get; set; }
}
