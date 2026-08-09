using Configuration;
using DbContext;
using DbModels;
using DbRepos;
using Models;
using Models.DTO;

namespace Services;

public class AuthService : IAuthService
{
    readonly JWTService _jwtService;
    readonly AuthDbRepo _repo;
    private Encryptions _encryptions;


    public AuthService(JWTService jwtService, AuthDbRepo repo, Encryptions encryptions)
    {
        _jwtService = jwtService;
        _repo = repo;
        _encryptions = encryptions;
    }
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _repo.GetByUsernameOrEmailAsync(request.UserNameOrEmail, request.UserNameOrEmail);

            if (user == null)
            {
                return new LoginResponse
                {
                    UserId = null,
                    UserName = null,
                    UserRole = null,
                    JwtToken = null
                };
            }
            
            var passwordResult = _encryptions.EncryptPasswordToBase64(request.Password) == user.PasswordHash;

            if (!passwordResult)
            {
                return new LoginResponse
                {
                    UserId = null,
                    UserName = null,
                    UserRole = null,
                    JwtToken = null
                };
            }

            var session = new LoginResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserRole = user.Role.ToString()
            };

            session.JwtToken = _jwtService.CreateJwtUserToken(session);

            return session;
        }
        catch
        {
            //if there was an error during login, simply pass it on.
            throw;
        }
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var existingUser = await _repo.GetByUsernameOrEmailAsync(request.UserName, request.Email);

            if (existingUser != null)
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Username or email is already in use."
                };
            }

            var passwordResult = _encryptions.EncryptPasswordToBase64(request.Password);


            var user = new UserDbM
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = passwordResult,
                FirstName = request.FirstName,
                Role = UserRolesEnum.User,
                Created_at = DateTime.UtcNow,
                Updated_at = DateTime.UtcNow,
                IsDeleted = false
            };

            await _repo.RegisterUserAsync(user);

            return new RegisterResponse
            {
                Success = true,
                Message = "Account created successfully."
            };
        }
        catch
        {
            throw;
        }
    }
}