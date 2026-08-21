namespace Models;
public interface IOrganizationInvitation
{
    Guid Id {get; set;}
    public IOrganization Organization {get; set;}
    string? InvitedEmail {get; set;}
    string Role {get; set;}
    string InviteCode {get; set;}
    DateTime ExpiresAt {get; set;}
    bool IsActive {get; set;}
    DateTime? AcceptedAt {get; set;}
    DateTime CreatedAt {get; set;}
    public IUser InvitedByUser {get;set;}
}