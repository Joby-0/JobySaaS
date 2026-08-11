using Microsoft.AspNetCore.Mvc;

namespace AppWebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SubscriptionController : ControllerBase
{
    readonly ILogger<SubscriptionController> _logger;
    public SubscriptionController(ILogger<SubscriptionController> logger)
    {
        _logger = logger;
    }

    

}