using Configuration;
using DbModels;
using DbRepos;
using Models;
using Models.DTO;

namespace Services;

public class SocialAccountService : ISocialAccountService
{
    readonly OrganizationDbRepo _orgRepo;
    readonly SocialAccountDbRepo _repo;
    private readonly Encryptions _encryptions;

    public SocialAccountService(SocialAccountDbRepo repo, OrganizationDbRepo orgRepo, Encryptions encryptions)
    {
        _repo = repo;
        _orgRepo = orgRepo;
        _encryptions = encryptions;
    }
    public async Task<ServiceResult<List<SocialAccountDto>>> GetConnectedAccountsAsync(Guid orgId, Guid requestUserId)
    {
        var userOrganization = await _orgRepo.GetUserOrganizationAsync(orgId, requestUserId);
        if (userOrganization == null)
        {
            return new ServiceResult<List<SocialAccountDto>>
            {
                Success = false,
                Message = "You do not have access to this organization."
            };
        }
        if (userOrganization.Role != "Owner" && userOrganization.Role != "Admin")
        {
            return new ServiceResult<List<SocialAccountDto>>
            {
                Success = false,
                Message = "You do not have access to this organization."
            };
        }

        var accounts = await _repo.GetByOrganizationIdAsync(orgId);

        return new ServiceResult<List<SocialAccountDto>>
        {
            Success = true,
            Data = accounts.Select(a => new SocialAccountDto
            {
                Id = a.Id,
                AccountName = a.Username,
                ProfileImageUrl = a.ProfileImageUrl,
                LastSync = a.LastSync,
                CostumUrl = a.CostumUrl,
                Followers = a.Followers,
                Status = a.Status,
                Platform = a.Platform
            }).ToList()
        };
    }



    //behöver någ inte den här varje service har sin egna men det kansek man inte vill
    public async Task<ServiceResult<SocialAccountDto>> AddSocialAccountAsync(Guid organizationId, Guid requestUserId, string platform, string username, string accessToken, string? refreshToken, DateTime? tokenExpiresAt)
    {
        var userOrganization = await _orgRepo.GetUserOrganizationAsync(organizationId, requestUserId);
        if (userOrganization is null)
        {
            return new ServiceResult<SocialAccountDto>
            {
                Success = false,
                Message = "You do not have access to this organization."
            };
        }

        if (userOrganization.Role != "Owner" && userOrganization.Role != "Admin")
        {
            return new ServiceResult<SocialAccountDto>
            {
                Success = false,
                Message = "You do not have access to this organization."
            };
        }
        if (Enum.TryParse<SocialAccountPlatfrom>(platform, true, out var result))
        {
            // result is SocialAccountStatus.Connected
        }

        var account = new SocialAccountDbM
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            Platform = result,
            Username = username,
            AccessToken = _encryptions.AesEncryptToBase64(accessToken),
            RefreshToken = refreshToken is not null ? _encryptions.AesEncryptToBase64(refreshToken) : null,
            TokenExpiresAt = tokenExpiresAt,
            // IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var saved = await _repo.AddAsync(account);

        return new ServiceResult<SocialAccountDto>
        {
            Success = true,
            Data = new SocialAccountDto
            {
                Id = saved.Id,
                AccountName = saved.Username,
                Platform = saved.Platform,
                // IsActive = saved.IsActive
            }
        };
    }
}