using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using Services;

namespace AppWebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
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
    [HttpPost("{organizationId:guid}")]
    [ActionName("subscription")]
    public async Task<IActionResult> CreateSubscriptionCheckout(Guid organizationId, [FromBody] SelectSubscriptionRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var requestUserId))
        {
            return Unauthorized();
        }
        var result = await _service.CreateSubscriptionCheckoutAsync(organizationId, request.SubscriptionId, requestUserId);

        return Ok(result);
    }

    [HttpGet]
    [ActionName("plans")]
    public async Task<IActionResult> GetSubscriptions()
    {
        var result = await _service.GetSubscriptionsAsync();

        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("{organizationId:guid}")]
    [ActionName("status")]
    [ProducesResponseType(200, Type = typeof(ServiceResult<OrganizationSubscriptionStatusDto>))]
    public async Task<IActionResult> GetSubscriptionStatus(Guid organizationId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var requestUserId))
        {
            return Unauthorized();
        }

        var result = await _service.GetSubscriptionStatusAsync(organizationId, requestUserId);
        return Ok(result);
    }

}