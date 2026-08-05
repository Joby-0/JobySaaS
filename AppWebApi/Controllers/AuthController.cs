using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using DbContext;

namespace AppWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    readonly JWTService _jwtService;

    public AuthController(JWTService jwtService)
    {
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("UserName and Password are required.");

        var user = new LoginResponse
        {
            UserId = Guid.NewGuid(),
            UserName = request.UserName,
            UserRole = "Admin"
        };

        var token = _jwtService.CreateJwtUserToken(user);
        return Ok(token);
    }
}
