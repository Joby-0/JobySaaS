using DbContext;
using DbModels;
// using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;

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
        var organization = await _dbContext.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == organizationId && o.UserOrganizationDbMs.Any(uo => uo.UserId == requestUserId));

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

    public async Task<List<OrganizationDto>> GetOrganizationsForUserAsync(Guid userId)
    {
        var organizations = await _dbContext.UserOrganizations
        .AsNoTracking()
        .Where(uo => uo.UserId == userId)
        .Select(uo => new OrganizationDto
        {
            Id = uo.Organization.Id,
            Name = uo.Organization.Name,
            OwnerId = uo.Organization.OwnerId,
            Role = uo.Role
        })
        .ToListAsync();

        return organizations;
    }

    public async Task<List<OrganizationMemberDTO>> GetMembers(Guid id)
    {
        var members = await _dbContext.UserOrganizations.AsNoTracking().Where(x => x.OrganizationId == id).Select(x => new OrganizationMemberDTO
        {
            UserId = x.UserId,
            OrganizationId = x.OrganizationId,
            Role = x.Role
        }).ToListAsync();
        return members;
    }

    public async Task RemoveOrganizationMemberAsync(Guid organizationId, Guid memberUserId)
    {
        var userOrganization = await _dbContext.UserOrganizations.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.UserId == memberUserId);
        if (userOrganization != null)
        {
            _dbContext.UserOrganizations.Remove(userOrganization);
            await _dbContext.SaveChangesAsync();
        }
    }
}