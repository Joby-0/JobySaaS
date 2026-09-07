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
    readonly IYoutubeService _youtubeService;
    private readonly Encryptions _encryptions;

    public SocialAccountService(SocialAccountDbRepo repo, OrganizationDbRepo orgRepo, Encryptions encryptions, IYoutubeService youtubeService)
    {
        _repo = repo;
        _orgRepo = orgRepo;
        _encryptions = encryptions;
        _youtubeService = youtubeService;
    }

    public async Task<ServiceResult<bool>> DisconnectAccountAsync(Guid orgId, Guid requestUserId, Guid accountId)
    {
        var userOrganization = await _orgRepo.GetUserOrganizationAsync(orgId, requestUserId);
        if (userOrganization == null)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                Message = "You do not have access to this organization."
            };
        }
        if (userOrganization.Role != "Owner" && userOrganization.Role != "Admin")
        {
            return new ServiceResult<bool>
            {
                Success = false,
                Message = "You do not have access to this organization."
            };
        }
        await _repo.DisconnectAccountAsync(accountId, orgId);

        return new ServiceResult<bool>
        {
            Success = true,
            Data = true
        };
    }

    public async Task<ServiceResult<SocialAccountDetails>> GetAccountDetailsAsync(Guid orgId, Guid requestUserId, Guid accountId)
    {
        var account = await _repo.GetSocialAccountByIdAsync(accountId);
        if (account == null)
            return ServiceResult<SocialAccountDetails>.Fail("Account was not found");

        var platform = account.Platform;

        ServiceResult<SocialAccountDetails> details = new ServiceResult<SocialAccountDetails>();
        if (platform == SocialPlatform.YouTube)
        {
            details = await _youtubeService.GetAccountDetailsAsync(account);
        }
        else if (platform == SocialPlatform.TikTok)
        {

        }
        else if (platform == SocialPlatform.Instagram)
        {

        }
        else if (platform == SocialPlatform.X)
        {

        }
        else if (platform == SocialPlatform.LinkedIn)
        {

        }
        else if (platform == SocialPlatform.Facebook)
        {

        }

        return details;
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
}