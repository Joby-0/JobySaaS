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
}