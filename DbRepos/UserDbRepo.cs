using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DbRepos;

public class UserDbRepo
{
    readonly MainDbContext _dbContext;

    public UserDbRepo(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IUser> GetUserAsync(Guid userId)
    {
        var user = await _dbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

        return user;
    }
}