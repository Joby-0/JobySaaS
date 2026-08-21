using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Services;

namespace AppWebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class InvitationController : ControllerBase
{
    private readonly IInvitationService _service;
    public InvitationController(IInvitationService invitationService)
    {
        _service = invitationService;
    }

    [Authorize]
    [HttpPost("{organizationId:guid}")]
    [ActionName("createinvitecode")]
    [ProducesResponseType(200, Type = typeof(ServiceResult<string>))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateInviteCode(Guid organizationId, [FromQuery] int expireInMinutes)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var requestUserId))
        {
            return Unauthorized();
        }

        var result = await _service.CreateInviteCodeAsync(organizationId, requestUserId, expireInMinutes);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet]
    [ActionName("createinvitecode")]
    [ProducesResponseType(200, Type = typeof(ServiceResult<InvitationPreviewDto>))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetInviteInfo([FromQuery] string code)
    {
        var result = await _service.GetInviteAsync(code);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}