using DbContext;
using DbModels;
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

    public async Task<IOrganization> CreateOrganizationAsync(OrganizationDbM organization)
    {
        try
        {
            _dbContext.Organizations.Add(organization);
            await _dbContext.SaveChangesAsync();
            return organization;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw new Exception($"Failed to create organization: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
    public async Task<IOrganization> GetOrganizationByIdAsync(Guid organizationId, Guid requestUserId)
    {
        var organization = await _dbContext.Organizations
        .AsNoTracking()
        .Include(x => x.Users)
        .FirstOrDefaultAsync(o => o.Id == organizationId && o.Users.Any(u => u.UserId == requestUserId));


        return organization;
    }
}