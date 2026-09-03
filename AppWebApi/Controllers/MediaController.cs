using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Services;

namespace AppWebApi.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _service;
    public MediaController(IMediaService mediaService)
    {
        _service = mediaService;
    }

    [Authorize]
    [HttpGet("{organizationId:guid}/list")]
    [ProducesResponseType(200, Type = typeof(ServiceResult<List<MediaListDTO>>))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetMediaList(Guid organizationId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var requestUserId))
        {
            return Unauthorized();
        }
        var result = await _service.GetMediaListAsync(organizationId, requestUserId, pageNumber, pageSize);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("{organizationId:guid}/media/{mediaId:guid}")]
    [ProducesResponseType(200, Type = typeof(ServiceResult<MediaDetailsDTO>))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetMediaDetails(Guid organizationId, Guid mediaId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var requestUserId))
        {
            return Unauthorized();
        }
        var result = await _service.GetMediaDetailsAsync(organizationId, mediaId, requestUserId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize]
    [HttpPost("{organizationId:guid}/media/upload")]
    [ProducesResponseType(200, Type = typeof(ServiceResult<Guid>))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateMedia(Guid organizationId, [FromBody] CreateMediaDTO mediaDetails)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var requestUserId))
        {
            return Unauthorized();
        }

        if (mediaDetails == null)
        {
            return BadRequest("Media details cannot be null.");
        }
        if(organizationId != mediaDetails.OrganizationId)
        {
            return BadRequest("Organization ID in the URL does not match the Organization ID in the request body.");
        }

        // Call the service to create the media
        var result = await _service.CreateMediaAsync(organizationId, mediaDetails, requestUserId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

}
