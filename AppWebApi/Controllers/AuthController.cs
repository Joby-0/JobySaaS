using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using DbContext;
using System.Text.RegularExpressions;
using Services;

namespace AppWebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    readonly ILogger<AuthController> _logger;
    readonly IAuthService _service;

    public AuthController(IAuthService service, ILogger<AuthController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost]
    [ActionName("Login")]
    [ProducesResponseType(200, Type = typeof(LoginResponse))]
    [ProducesResponseType(400, Type = typeof(string))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("LoginUser initiated");

        try
        {
            // Note: Validate userCreds to avoid sql injection

            var pUsername = @"^(?=.{3,20}$)[a-zA-Z0-9](?:[a-zA-Z0-9._]*[a-zA-Z0-9])$";
            var pEmail = @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
            var pPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,64}$";

            // Username OR Email
            var userRegex = new Regex($"({pUsername})|({pEmail})", RegexOptions.IgnoreCase);
            if (!userRegex.IsMatch(request.UserNameOrEmail))
                throw new ArgumentException("Wrong username or email format");

            // Password
            var passRegex = new Regex(pPassword);
            if (!passRegex.IsMatch(request.Password))
                throw new ArgumentException("Wrong password format");

            //With validated credentials proceed to login
            var _usr = await _service.LoginAsync(request);
            _logger.LogInformation($"{_usr.UserName} logged in");
            return Ok(_usr);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Login Error: {ex.Message}");
            return BadRequest($"Login Error: {ex.Message}");
        }
    }


    [HttpPost]
    [ActionName("Register")]
    [ProducesResponseType(200, Type = typeof(RegisterResponse))]
    [ProducesResponseType(400, Type = typeof(string))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("RegisterUser initiated");

        try
        {
            // Strong password regex
            var pStrong = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,64}$";

            // RFC2822 email pattern
            var pEmail = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

            // Username (letters + numbers only, 4–20 chars)
            var pUserName = @"^[a-zA-Z0-9]{4,20}$";

            // Validate username
            if (!Regex.IsMatch(request.UserName, pUserName))
                throw new ArgumentException("Wrong username format");

            // Validate email
            if (!Regex.IsMatch(request.Email, pEmail, RegexOptions.IgnoreCase))
                throw new ArgumentException("Wrong email format");

            // Validate password
            if (!Regex.IsMatch(request.Password, pStrong))
                throw new ArgumentException("Wrong password format");

            // Call service
            var result = await _service.RegisterAsync(request);

            if (!result.Success)
                throw new Exception("User registration failed");

            _logger.LogInformation($"{request.UserName} registered successfully");

            return Created("", result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Register Error: {ex.Message}");
            return BadRequest($"Register Error: {ex.Message}");
        }
    }
}
