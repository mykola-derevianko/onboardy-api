using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.Attributes;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Extensions;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Controllers
{
    [ApiController]
    [Route("api/organizations/{orgId}/invitations")]
    [Authorize]
    public class OrganizationInvitationsController : ControllerBase
    {
        private readonly IInvitationService _invitationService;
        private readonly IMapperService _mapperService;

        public OrganizationInvitationsController(IInvitationService invitationService, IMapperService mapperService)
        {
            _invitationService = invitationService;
            _mapperService = mapperService;
        }

        [HttpPost]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        public async Task<ActionResult<InvitationResponse>> Create(CreateInvitationRequest request, Guid orgId)
        {
            var invitedByUserId = User.GetUserId();
            var result = await _invitationService.CreateAsync(orgId, invitedByUserId, request);

            if (result.IsFailure)
                return this.ToProblem(result.Error);

            return CreatedAtAction(
                nameof(Accept),
                new { token = result.Value.Token },
                _mapperService.ToInvitationResponse(result.Value));
        }

        [HttpGet]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        public async Task<ActionResult<IReadOnlyCollection<InvitationResponse>>> GetByOrganization(Guid orgId)
        {
            var result = await _invitationService.GetByOrganizationAsync(orgId);

            if (result.IsFailure)
                return this.ToProblem(result.Error);

            return Ok(result.Value.Select(_mapperService.ToInvitationResponse).ToList());
        }

        [HttpDelete("{invitationId:guid}")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        public async Task<IActionResult> Delete(Guid orgId, Guid invitationId)
        {
            var result = await _invitationService.DeleteAsync(orgId, invitationId);

            if (result.IsFailure)
                return this.ToProblem(result.Error);

            return NoContent();
        }

        [HttpPost("/api/invitations/{token}/accept")]
        public async Task<ActionResult<AcceptInvitationResponse>> Accept(string token)
        {
            var currentUserId = User.GetUserId();
            var result = await _invitationService.AcceptAsync(currentUserId, token);

            if (result.IsFailure)
                return this.ToProblem(result.Error);

            return Ok(new AcceptInvitationResponse
            {
                Success = true
            });
        }
    }
}