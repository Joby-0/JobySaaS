namespace Services;

public interface IInvitationService
{
    Task<ServiceResult<string>> CreateInviteCodeAsync(Guid organizationId, Guid requestUserId,int expireInMinutes);
    
}