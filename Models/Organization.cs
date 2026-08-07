namespace Models;

public class Organization : IOrganization
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid OwnerId { get; set; }
    public ISubscription Subscription { get; set; } = null!;
    public List<IUserOrganization> Users { get; set; } = new();
    public List<ISocialAccount> SocialAccounts { get; set; } = new();
    public List<IPost> Posts { get; set; } = new();
    public List<IMedia> Media { get; set; } = new();
}
