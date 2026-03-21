using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Exceptions.Identity;
using OnBoardy.API.Extensions;
using OnBoardy.API.Models;
using OnBoardy.API.Services.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
            return CreatedAtAction(nameof(GetById), new { id = organization.Id }, Map(organization));
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<OrganizationResponseDTO>>> GetMyOrganizations()
        {
            var currentUserId = User.GetUserId();

            var organizations = await _organizationService.GetAllByUserIdAsync(currentUserId);
            return Ok(organizations.Select(Map).ToList());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrganizationResponseDTO>> GetById(Guid id)
        {
            var currentUserId = User.GetUserId();

            var organizations = await _organizationService.GetAllByUserIdAsync(currentUserId);
            var organization = organizations.FirstOrDefault(x => x.Id == id)
                ?? throw new OrganizationNotFoundException();

            return Ok(Map(organization));
        }

        [HttpPatch("{id:guid}")]
        [TypeFilter(typeof(AuthorizeRoleFilter), Arguments = new object[] { MembershipRole.Owner })]
        public async Task<ActionResult<OrganizationResponseDTO>> Update(Guid id, UpdateOrganizationRequestDTO request)
        {
            var currentUserId = User.GetUserId();
            var organization = await _organizationService.UpdateAsync(id, request);
            return Ok(Map(organization));
        }

        [HttpDelete("{id:guid}")]
        [TypeFilter(typeof(AuthorizeRoleFilter), Arguments = new object[] { MembershipRole.Owner })]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentUserId = User.GetUserId();

            await _organizationService.DeleteAsync(id);
            return NoContent();
        }

        private static OrganizationResponseDTO Map(Organization organization)
        {
            return new OrganizationResponseDTO
            {
                Id = organization.Id,
                Name = organization.Name,
                Description = organization.Description,
                CreatedAt = organization.CreatedAt,
                UpdatedAt = organization.UpdatedAt
            };
        }
    }
}