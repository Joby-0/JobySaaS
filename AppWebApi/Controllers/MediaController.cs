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
    public async Task<IActionResult> CreateMedia(Guid organizationId, [FromForm] CreateMediaDTO createMediaDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var requestUserId))
        {
            return Unauthorized();
        }


        if (createMediaDto.Video == null || createMediaDto.Video.Length == 0)
        {
            return BadRequest("No media file was uploaded.");
        }
        var file = createMediaDto.Video;
        var title = createMediaDto.Title;
        var description = createMediaDto.Description;

        var result = await _service.CreateMediaAsync(organizationId, file, title, description, requestUserId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }


    [Authorize]
    [HttpPost("{organizationId:guid}/media/{mediaId:guid}/publish")]
    [ProducesResponseType(200, Type = typeof(ServiceResult<bool>))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PublishMedia(Guid organizationId, Guid mediaId, [FromBody] List<Guid> socialAccountIds)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var requestUserId))
        {
            return Unauthorized();
        }

        var result = await _service.PublishMediaAsync(organizationId, mediaId, socialAccountIds, requestUserId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
