using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace AppWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class YoutubeController : ControllerBase
{
    readonly IYoutubeService _service;
    readonly ILogger<YoutubeController> _logger;

    public YoutubeController(IYoutubeService service, ILogger<YoutubeController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [Authorize]
    [HttpGet("connect")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(302, Type = typeof(string))]
    [ProducesResponseType(400, Type = typeof(string))]
    public async Task<IActionResult> Connect([FromQuery] Guid organizationId)
    {
        var requestUserId = GetUserIdFromClaims();
        if (requestUserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await _service.Connect(organizationId);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result.Data.ToString());
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
    {
        var result = await _service.Callback(code, state);

        if (!result.Success)
        {
            return Redirect($"{"localhost:5055"}/connect?youtube=error&message={Uri.EscapeDataString(result.Message)}");
        }

        return Redirect($"{"localhost:5055"}/org/{result.Data}/social-accounts?youtube=success");
    }

    [Authorize]
    [HttpPost("publish")]
    public async Task<IActionResult> Publish([FromForm] Guid mediaId, [FromForm] string title, [FromForm] string description, [FromForm] string categoryId, [FromForm] string userId)
    {
        var requestUserId = GetUserIdFromClaims();
        if (requestUserId == Guid.Empty)
        {
            return Unauthorized();
        }
        if (!Guid.TryParse(userId, out var accountId))
        {
            return BadRequest("A valid selected YouTube account userId is required.");
        }
        var result = await _service.UploadVideoAsync(mediaId, title, description, categoryId, accountId, requestUserId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    private Guid GetUserIdFromClaims()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var requestUserId))
        {
            throw new UnauthorizedAccessException("Invalid user ID.");
        }
        return requestUserId;
    }
}
