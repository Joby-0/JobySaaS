using Microsoft.AspNetCore.Mvc;
using Services;

namespace AppWebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class StripeController : ControllerBase
{
    readonly ILogger<SubscriptionController> _logger;
    readonly IStripeService _service;
    public StripeController(ILogger<SubscriptionController> logger, IStripeService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost]
    [ActionName("webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        using var reader = new StreamReader(HttpContext.Request.Body);

        var json = await reader.ReadToEndAsync();

        var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();

        if (string.IsNullOrEmpty(signature))
        {
            return BadRequest();
        }

        // Send json + signature to StripeService
        var result = await _service.HandleWebhookAsync(json,signature);

        if (!result)
        {
            return BadRequest();
        }

        return Ok();
    }


}