using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AppWebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/organizations/{organizationId:guid}/publish-jobs")]
public sealed class PublishJobsController : ControllerBase
{
    private readonly IPublishJobService _service;

    public PublishJobsController(IPublishJobService service) => _service = service;

    [HttpGet("{jobId:guid}")]
    public async Task<IActionResult> Get(Guid organizationId, Guid jobId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var result = await _service.GetStatusAsync(organizationId, jobId, userId);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }
}
