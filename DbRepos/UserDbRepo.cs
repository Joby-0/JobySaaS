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
    public async Task<UserDbM> EnsureUserExistsAsync(Guid userId, string userName, string email, string? profileImageUrl = null, string? firstName = null, string? lastName = null)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user is not null) return user;

        user = new UserDbM
        {
            Id = userId,
            UserName = userName,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ProfileImage = profileImageUrl,
            FirstName = firstName,
            LastName = lastName
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }
}