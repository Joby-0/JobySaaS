namespace Models;

public class OrganizationInvitation : IOrganizationInvitation
{
    public virtual Guid Id { get; set; }
    public string? InvitedEmail { get; set; }
    public string Role { get; set; }
    public string InviteCode { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public virtual IOrganization Organization { get; set; }
    public virtual IUser InvitedByUser { get; set; }
}
