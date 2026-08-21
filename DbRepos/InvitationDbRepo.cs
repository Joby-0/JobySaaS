using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;

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
    public async Task<OrganizationInvitationDbM> GetInviteAsync(string inviteCode)
    {
       var code = await _dbContext.OrganizationInvitations.Include(x => x.UserDbM).Include(x =>x.OrganizationDbM).Where(x => x.InviteCode == inviteCode).FirstOrDefaultAsync();
       return code;
    }

}