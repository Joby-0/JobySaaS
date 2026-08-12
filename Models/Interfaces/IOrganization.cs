namespace Models;

public interface IOrganization
{
    Guid Id { get; set; }
    string Name { get; set; }
    DateTime CreatedAt { get; set; }
    Guid OwnerId { get; set; }


    IOrganizationSubscription? OrganizationSubscription { get; set; }
    List<IUserOrganization> Users { get; set; }
    List<ISocialAccount> SocialAccounts { get; set; }
    List<IPost> Posts { get; set; }
    List<IMedia> Media { get; set; }

}