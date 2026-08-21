namespace Models;

public class OrganizationInvitation : IOrganizationInvitation
{
    public virtual Guid Id { get; set; }
    public virtual Guid OrganizationId { get; set; }
    public string? InvitedEmail { get; set; }
    public string Role { get; set; }
    public string InviteCode { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual Guid InvitedByUserId { get; set; }
    public bool IsAvtice { get; set; }
}
