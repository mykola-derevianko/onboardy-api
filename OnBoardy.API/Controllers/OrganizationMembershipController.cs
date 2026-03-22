using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Extensions;
using OnBoardy.API.Services.Infrastructure;
using System.Net;

namespace OnBoardy.API.Controllers
{
    [ApiController]
    [Route("api/organizations/{orgId}/memberships")]
    [Authorize]
    public class OrganizationMembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public OrganizationMembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpGet]
        public async Task<ActionResult<MembershipResponse>> GetCurrentUserMembership(Guid orgId)
        {
            var currentUserId = User.GetUserId();

            var memberships = await _membershipService.GetAllByUserIdAsync(currentUserId);
            var membership = memberships.FirstOrDefault(x => x.OrganizationId == orgId)
                ?? throw new DomainException("Membership not found.", HttpStatusCode.NotFound);

            return Ok(membership.ToResponseDTO());
        }
            
        [HttpGet("all")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        public async Task<ActionResult<IReadOnlyCollection<MembershipResponse>>> GetAllByOrganization(Guid orgId)
        {
            var memberships = await _membershipService.GetAllByOrganizationIdAsync(orgId);
            return Ok(memberships.Select(x => x.ToResponseDTO()).ToList());
        }
    }
}