using DbContext;
using DbModels;

namespace DbRepos;

public class InvitationDbRepo
{
    readonly MainDbContext _dbContext;

    public InvitationDbRepo(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateInviteCodeAsync(OrganizationInvitationDbM inviteCode)
    {
        _dbContext.OrganizationInvitations.Add(inviteCode);
        await _dbContext.SaveChangesAsync();

    }

}