using System.Security.Cryptography;
using DbModels;
using DbRepos;
using Models.DTO;

namespace Services;

public class InvitationService : IInvitationService
{
    readonly InvitationDbRepo _repo;
    readonly OrganizationDbRepo _orgRepo;
    public InvitationService(InvitationDbRepo repo, OrganizationDbRepo orgRepo)
    {
        _repo = repo;
        _orgRepo = orgRepo;
    }
    public async Task<ServiceResult<string>> CreateInviteCodeAsync(Guid organizationId, Guid requestUserId, int expireInMinutes)
    {
        var userOrganization = await _orgRepo.GetUserOrganizationAsync(organizationId, requestUserId);

        if (userOrganization == null)
        {
            return ServiceResult<string>.Fail("You are not a member of this organization.");
        }

        if (userOrganization.Role != "Owner" && userOrganization.Role != "Admin")
        {
            return ServiceResult<string>.Fail("You are not authorized to create invite codes.");
        }

        // Only allow specific expiration times
        if (expireInMinutes != 5 && expireInMinutes != 15 && expireInMinutes != 60 && expireInMinutes != 1440)
        {
            return ServiceResult<string>.Fail("Invalid expiration time. Allowed values are 5, 15, 60 or 1440 minutes.");
        }

        var now = DateTime.UtcNow;
        var code = GenerateInviteCode();

        var inviteCode = new OrganizationInvitationDbM
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            InviteCode = code,
            InvitedByUserId = requestUserId,
            Role = "Member",
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(expireInMinutes),
            AcceptedAt = null,
            IsActive = true
        };

        await _repo.CreateInviteCodeAsync(inviteCode);

        return ServiceResult<string>.Ok("Invite code created successfully.", code);
    }

    public async Task<ServiceResult<InvitationPreviewDto>> GetInviteAsync(string inviteCode)
    {
        var code = await _repo.GetInviteAsync(inviteCode);

        if (code == null)
        {
            return new ServiceResult<InvitationPreviewDto>
            {
                Success = false,
                Error = "The invitation code was not found."
            };
        }

        if (!code.IsActive)
        {
            return new ServiceResult<InvitationPreviewDto>
            {
                Success = false,
                Error = "This invitation code is no longer active."
            };
        }

        if (code.ExpiresAt <= DateTime.UtcNow)
        {
            return new ServiceResult<InvitationPreviewDto>
            {
                Success = false,
                Error = "This invitation code has expired. Please ask the organization administrator for a new invitation."
            };
        }

        return new ServiceResult<InvitationPreviewDto>
        {
            Success = true,
            Data = new InvitationPreviewDto
            {
                Organization = code.OrganizationDbM.Name,
                InvitedBy = code.UserDbM.UserName,
                ExpireAt = code.ExpiresAt
            }
        };
    }

    private string GenerateInviteCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        var bytes = RandomNumberGenerator.GetBytes(8);

        var code = new char[8];

        for (int i = 0; i < code.Length; i++)
        {
            code[i] = chars[bytes[i] % chars.Length];
        }

        return $"{new string(code, 0, 4)}-{new string(code, 4, 4)}";
    }



    //todo
    //POST /api/invitation/{code}/accept
}