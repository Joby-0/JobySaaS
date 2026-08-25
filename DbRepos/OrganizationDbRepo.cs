using DbContext;
using DbModels;
// using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DbRepos;

public class OrganizationDbRepo
{
    readonly MainDbContext _dbContext;

    public OrganizationDbRepo(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IOrganization> GetOrganizationByIdAsync(Guid organizationId, Guid requestUserId)
    {
        var organization = await _dbContext.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == organizationId &&
        _dbContext.UserOrganizations.Any(uo => uo.OrganizationId == o.Id && uo.UserId == requestUserId));

        return organization;
    }
    public async Task<IOrganization> CreateOrganizationAsync(OrganizationDbM organization)
    {
        try
        {
            _dbContext.Organizations.Add(organization);
            _dbContext.UserOrganizations.Add(new UserOrganizationDbM
            {
                UserId = organization.OwnerId,
                OrganizationId = organization.Id,
                Role = "Owner",
                CreatedAt = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();
            return organization;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw new Exception($"Failed to create organization: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
    public async Task<IUserOrganization> GetUserOrganizationAsync(Guid organizationId, Guid requestUserId)
    {
        var role = await _dbContext.UserOrganizations.Where(x => x.UserId == requestUserId && x.OrganizationId == organizationId).FirstOrDefaultAsync();

        return role;
    }

    public async Task AddUserToOrganization(UserOrganizationDbM user)
    {
        _dbContext.UserOrganizations.Add(user);

        await _dbContext.SaveChangesAsync();
    }
}