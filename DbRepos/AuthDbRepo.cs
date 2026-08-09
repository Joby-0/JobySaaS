using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Configuration;
using DbContext;
using DbModels;

namespace DbRepos;

public class AuthDbRepo
{
    private readonly ILogger<AuthDbRepo> _logger;
    private readonly MainDbContext _dbContext;

    public AuthDbRepo(ILogger<AuthDbRepo> logger, MainDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<UserDbM?> GetByUsernameOrEmailAsync(string username, string email) => await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == username || x.Email == email);
    public async Task RegisterUserAsync(UserDbM request)
    {
        try
        {
            await _dbContext.Users.AddAsync(request);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            throw;
        }
    }
}

