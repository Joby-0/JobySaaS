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

    [HttpGet("connect")]
    public async Task<IActionResult> Connect()
    {
        var result = await _service.Connect();
        if (!result.Success)
            return BadRequest(result);

        return Ok(result.Data.ToString());
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string scope)
    {
        var result = await _service.Callback(code);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile video, [FromForm] string title, [FromForm] string description, [FromForm] string categoryId, [FromForm] ISocialAccount socialAccount)
    {
        if (video == null || video.Length == 0)
        {
            return BadRequest(ServiceResult.Fail("No video was uploaded."));
        }
        if (!video.ContentType.StartsWith("video/"))
        {
            return BadRequest(ServiceResult.Fail("The uploaded file is not a video."));
        }

        var result = await _service.UploadVideoAsync(video, title, description, categoryId, socialAccount);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
