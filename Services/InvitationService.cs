using System.Security.Cryptography;
using DbModels;
using DbRepos;

namespace Services;

public class InvitationService : IInvitationService
{
    readonly InvitationDbRepo _repo;
    readonly OrganizationDbRepo _orgRepo;
    public InvitationService(InvitationDbRepo organizationDbRepo)
    {
        _repo = organizationDbRepo;
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
            IsAvtice = true
        };

        await _repo.CreateInviteCodeAsync(inviteCode);

        return ServiceResult<string>.Ok("Invite code created successfully.", code);
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
    //GET /api/invitation/{code}
    //POST /api/invitation/{code}/accept
}