namespace AppWebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DbModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models;
    using Models.DTO;
    using Services;

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OrganizationController : ControllerBase
    {
        private readonly ILogger<OrganizationController> _logger;
        private readonly IOrganizationService _organizationService;

        public OrganizationController(ILogger<OrganizationController> logger, IOrganizationService organizationService)
        {
            _logger = logger;
            _organizationService = organizationService;
        }

        [Authorize]
        [HttpPost]
        [ActionName("CreateOrganization")]
        [ProducesResponseType(200, Type = typeof(IOrganization))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var ownerId))
            {
                return Unauthorized();
            }
            var result = await _organizationService.CreateOrganizationAsync(request, ownerId);
            return CreatedAtAction(nameof(GetOrganization), new { organizationId = result.Id }, result);
        }

        [Authorize]
        [HttpPost("{organizationId:guid}")]
        [ActionName("GetOrganization")]
        [ProducesResponseType(200, Type = typeof(IOrganization))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> GetOrganization(Guid organizationId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out var requestUserId))
            {
                return Unauthorized();
            }

            var organization = await _organizationService.GetOrganizationByIdAsync(organizationId, requestUserId);

            if (organization == null)
            {
                return NotFound();
            }
            return Ok(organization);
        }



        //Todo 
        //GET /organization/{id}/members
        //GET /organization/{id}/social-accounts
        //GET /organization/{id}/subscription
        //GET /organization/{id}/posts
        //GET /organization/{id}/analytics
    }
}