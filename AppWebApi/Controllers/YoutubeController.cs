using Microsoft.AspNetCore.Mvc;
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
}
