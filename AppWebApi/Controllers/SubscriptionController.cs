using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using Services;

namespace AppWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    readonly ILogger<SubscriptionController> _logger;
    readonly ISubscriptionService _service;
    public SubscriptionController(ILogger<SubscriptionController> logger, ISubscriptionService service)
    {
        _logger = logger;
        _service = service;
    }

    [Authorize]
    [HttpPost("{organizationId:guid}/subscription")]
    public async Task<IActionResult> CreateSubscriptionCheckout(Guid organizationId, [FromBody] SelectSubscriptionRequest request)
    {
        var requestUserId = GetUserIdFromClaims();
        if (requestUserId == Guid.Empty)
        {
            return Unauthorized();
        }
        var result = await _service.CreateSubscriptionCheckoutAsync(organizationId, request.SubscriptionId, requestUserId);

        return Ok(result);
    }

    [HttpGet("plans")]
    public async Task<IActionResult> GetSubscriptions()
    {
        var result = await _service.GetSubscriptionsAsync();

        return Ok(result);
    }

    [Authorize]
    [HttpGet("{organizationId:guid}/status")]
    [ProducesResponseType(200, Type = typeof(ServiceResult<OrganizationSubscriptionStatusDto>))]
    public async Task<IActionResult> GetSubscriptionStatus(Guid organizationId)
    {
        var requestUserId = GetUserIdFromClaims();
        if (requestUserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await _service.GetSubscriptionStatusAsync(organizationId, requestUserId);
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