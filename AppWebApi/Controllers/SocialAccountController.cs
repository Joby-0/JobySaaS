using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using Services;

namespace AppWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class SocialAccountController : ControllerBase
{
    private readonly ISocialAccountService _service;

    public SocialAccountController(ISocialAccountService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet("{organizationId:guid}/mine")]
    public async Task<IActionResult> GetConnectedAccounts(Guid organizationId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var requestUserId)) return Unauthorized();

        var result = await _service.GetConnectedAccountsAsync(organizationId, requestUserId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{organizationId:guid}/disconnect")]
    public async Task<IActionResult> DisconnectAccount(Guid organizationId, [FromQuery] Guid accountId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var requestUserId)) return Unauthorized();

        var result = await _service.DisconnectAccountAsync(organizationId, requestUserId, accountId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}