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
    [Route("api/[controller]")]
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
        [HttpPost("create")]
        [ProducesResponseType(200, Type = typeof(ServiceResult<OrganizationDto>))]
        [ProducesResponseType(400, Type = typeof(ServiceResult<OrganizationDto>))]
        public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst("UserName")?.Value;
            var email = User.FindFirst("Email")?.Value;

            if (!Guid.TryParse(userId, out var ownerId) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var result = await _organizationService.CreateOrganizationAsync(request, ownerId, userName, email);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{organizationId:guid}/get")]
        [ProducesResponseType(200, Type = typeof(ServiceResult<OrganizationDto>))]
        [ProducesResponseType(400, Type = typeof(ServiceResult<OrganizationDto>))]
        public async Task<IActionResult> GetOrganization(Guid organizationId)
        {
            var requestUserId = GetUserIdFromClaims();

            var result = await _organizationService.GetOrganizationByIdAsync(organizationId, requestUserId);

            if (result == null)
            {
                return NotFound();
            }
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("mine")]
        [ProducesResponseType(200, Type = typeof(ServiceResult<List<OrganizationDto>>))]
        public async Task<IActionResult> GetMyOrganizations()
        {
            var requestUserId = GetUserIdFromClaims();

            var result = await _organizationService.GetOrganizationsForUserAsync(requestUserId);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}/members")]
        [ProducesResponseType(200, Type = typeof(ServiceResult<List<OrganizationMemberDTO>>))]
        [ProducesResponseType(400, Type = typeof(ServiceResult<List<OrganizationMemberDTO>>))]
        public async Task<IActionResult> GetMembers(Guid id)
        {
            var requestUserId = GetUserIdFromClaims();

            var result = await _organizationService.GetOrganizationMembersAsync(id, requestUserId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}/members/{userId}/remove")]
        [ProducesResponseType(200, Type = typeof(ServiceResult<string>))]
        [ProducesResponseType(400, Type = typeof(ServiceResult<string>))]
        public async Task<IActionResult> RemoveMember(Guid id, Guid userId)
        {
            var requestUserId = GetUserIdFromClaims();

            if (requestUserId == Guid.Empty)
            {
                return Unauthorized();
            }

            var result = await _organizationService.RemoveOrganizationMemberAsync(id, userId, requestUserId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        //Todo
        //Delete /organization/{id}/delete
        //Update /organization/{id}/update


        //GET /organization/{id}/posts
        //GET /organization/{id}/analytics

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
}