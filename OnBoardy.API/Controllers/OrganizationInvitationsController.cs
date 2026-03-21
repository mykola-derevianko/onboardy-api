using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Extensions;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Controllers
{
    [ApiController]
    [Route("api/organization/{orgId}/invitation")]
    [Authorize]
    public class OrganizationInvitationsController : ControllerBase
    {
        private readonly IInvitationService _invitationService;

        public OrganizationInvitationsController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpPost]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        public async Task<ActionResult<InvitationResponseDTO>> Create(CreateInvitationRequestDTO request, Guid orgId)
        {
            var invitedByUserId = User.GetUserId();
            var invitation = await _invitationService.CreateAsync(orgId, invitedByUserId, request);

            return CreatedAtAction(
                nameof(Accept),
                new { token = invitation!.Token },
                invitation!.ToResponseDTO());
        }

        [HttpGet]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        public async Task<ActionResult<IReadOnlyCollection<InvitationResponseDTO>>> GetByOrganization(Guid orgId)
        {
            var invitations = await _invitationService.GetByOrganizationAsync(orgId);
            return Ok(invitations.Select(x => x.ToResponseDTO()).ToList());
        }

        [HttpDelete("{invitationId:guid}")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        public async Task<IActionResult> Delete(Guid orgId, Guid invitationId)
        {
            await _invitationService.DeleteAsync(orgId, invitationId);
            return NoContent();
        }

        [HttpPost("/api/invitations/{token}/accept")]
        public async Task<ActionResult<AcceptInvitationResponseDTO>> Accept(string token)
        {
            var currentUserId = User.GetUserId();
            var result = await _invitationService.AcceptAsync(currentUserId, token);

            return Ok(new AcceptInvitationResponseDTO
            {
                Success = result
            });
        }
    }
}