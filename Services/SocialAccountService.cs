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
            return ServiceResult<bool>.Fail( "You do not have access to this organization.");
        }
        if (userOrganization.Role != "Owner" && userOrganization.Role != "Admin")
        {
            return ServiceResult<bool>.Fail( "You do not have access to this organization.");
        }
        await _repo.DisconnectAccountAsync(accountId, orgId);

        return  ServiceResult<bool>.Ok("", true);
    }

    public async Task<ServiceResult<SocialAccountDetails>> GetAccountDetailsAsync(Guid orgId, Guid requestUserId, Guid accountId)
    {
        var userOrganization = await _orgRepo.GetUserOrganizationAsync(orgId, requestUserId);
        if (userOrganization == null)
        {
            return  ServiceResult<SocialAccountDetails>.Fail("You do not have access to this organization.");
        }
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
    public async Task<ServiceResult<List<DailyMetricDto>>> GetAccountPerformanceAsync(Guid orgId, Guid requestUserId, Guid accountId, DateOnly startDate, DateOnly endDate, CancellationToken ct)
    {
        var userOrganization = await _orgRepo.GetUserOrganizationAsync(orgId, requestUserId);
        if (userOrganization == null)
        {
            return  ServiceResult<List<DailyMetricDto>>.Fail("You do not have access to this organization.");
        }

        var account = await _repo.GetSocialAccountByIdAsync(accountId);
        if (account == null)
            return ServiceResult<List<DailyMetricDto>>.Fail("Account was not found");

        var platform = account.Platform;

        ServiceResult<List<DailyMetricDto>> details = new ServiceResult<List<DailyMetricDto>>();
        if (platform == SocialPlatform.YouTube)
        {
            details = await _youtubeService.GetAccountPerformanceAsync(account, startDate, endDate, ct);
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

    public async Task<ServiceResult<PagedResult<RecentVideoDto>>> GetAccountVideosAsync(Guid organizationId, Guid requestUserId, Guid accountId, int pageNumber, int PageSize, string order, CancellationToken ct)
    {
        var account = await _repo.GetSocialAccountByIdAsync(accountId);
        if (account == null)
            return ServiceResult<PagedResult<RecentVideoDto>>.Fail("Account was not found");

        var platform = account.Platform;

        ServiceResult<PagedResult<RecentVideoDto>> videos = new ServiceResult<PagedResult<RecentVideoDto>>();
        if (platform == SocialPlatform.YouTube)
        {
            videos = await _youtubeService.GetAccountVideosAsync(account, pageNumber, PageSize,order, ct);
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

        return videos;
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