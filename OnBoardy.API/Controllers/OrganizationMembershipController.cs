using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Extensions;
using OnBoardy.API.Results;
using OnBoardy.API.Services.Infrastructure;

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

            var membershipsResult = await _membershipService.GetAllByUserIdAsync(currentUserId);
            if (membershipsResult.IsFailure)
                return this.ToProblem(membershipsResult.Error);

            var membership = membershipsResult.Value.FirstOrDefault(x => x.OrganizationId == orgId);
            if (membership is null)
                return this.ToProblem(MembershipErrors.NotFound);

            return Ok(membership.ToResponseDTO());
        }

        [HttpGet("all")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        public async Task<ActionResult<IReadOnlyCollection<MembershipResponse>>> GetAllByOrganization(Guid orgId)
        {
            var result = await _membershipService.GetAllByOrganizationIdAsync(orgId);
            if (result.IsFailure)
                return this.ToProblem(result.Error);

            return Ok(result.Value.Select(x => x.ToResponseDTO()).ToList());
        }
    }
}