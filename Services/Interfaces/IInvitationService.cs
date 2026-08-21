using DbModels;
using Models.DTO;

namespace Services;

public interface IInvitationService
{
    Task<ServiceResult<string>> CreateInviteCodeAsync(Guid organizationId, Guid requestUserId,int expireInMinutes);
    
    Task<ServiceResult<InvitationPreviewDto>> GetInviteAsync(string inviteCode);
}