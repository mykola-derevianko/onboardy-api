using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Extensions;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpPost]
        public async Task<ActionResult<OrganizationResponseDTO>> Create(CreateOrganizationRequestDTO request)
        {
            var currentUserId = User.GetUserId();

            var organization = await _organizationService.CreateAsync(request, currentUserId);
            return CreatedAtAction(nameof(GetById), new { orgId = organization.Id }, organization.ToResponseDTO());
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<OrganizationResponseDTO>>> GetMyOrganizations()
        {
            var currentUserId = User.GetUserId();

            var organizations = await _organizationService.GetAllByUserIdAsync(currentUserId);
            return Ok(organizations.Select(x => x.ToResponseDTO()).ToList());
        }

        [HttpGet("{orgId:guid}")]
        public async Task<ActionResult<OrganizationResponseDTO>> GetById(Guid orgId)
        {
            var currentUserId = User.GetUserId();

            var organizations = await _organizationService.GetAllByUserIdAsync(currentUserId);
            var organization = organizations.FirstOrDefault(x => x.Id == orgId)
                ?? throw new OrganizationNotFoundException();

            return Ok(organization.ToResponseDTO());
        }

        [HttpPatch("{orgId:guid}")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        public async Task<ActionResult<OrganizationResponseDTO>> Update(Guid orgId, UpdateOrganizationRequestDTO request)
        {
            var currentUserId = User.GetUserId();
            var organization = await _organizationService.UpdateAsync(orgId, request);
            return Ok(organization.ToResponseDTO());
        }

        [HttpDelete("{orgId:guid}")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]

        public async Task<IActionResult> Delete(Guid orgId)
        {
            var currentUserId = User.GetUserId();

            await _organizationService.DeleteAsync(orgId);
            return NoContent();
        }
    }
}