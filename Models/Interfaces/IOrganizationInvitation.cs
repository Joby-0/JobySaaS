namespace Models;
public interface IOrganizationInvitation
{
    Guid Id {get; set;}
    Guid OrganizationId {get; set;}
    string? InvitedEmail {get; set;}
    string Role {get; set;}
    string InviteCode {get; set;}
    DateTime ExpiresAt {get; set;}
    bool IsAvtice {get; set;}
    DateTime? AcceptedAt {get; set;}
    DateTime CreatedAt {get; set;}
    Guid InvitedByUserId {get; set;}
}