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

        public OrganizationInvitationsController(IInvitationService invitationService) {
            _invitationService = invitationService;
        }
        
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        [HttpPost]
        public async Task<IActionResult> Create(CreateInvitationRequestDTO request, Guid orgId) {
            var invitedByUserId = User.GetUserId();

            var invitation = await _invitationService.CreateAsync(orgId, invitedByUserId, request);


            //TODO: Send email notification to the invited user with the invitation link, if email is provided;
            //  return link in response body
            return CreatedAtAction(
                nameof(Accept),
                new { orgId, token = invitation!.Token },
                new
                {
                    invitation.Id,
                    invitation.Token,
                    invitation.ExpiresAt
                });
        }

        //[HttpGet]
        //public async Task<IActionResult> GetByOrganization(Guid orgId, [FromQuery] InvitationStatus? status) { }

        //[HttpDelete("{invitationId}")]
        //public async Task<IActionResult> Delete(Guid orgId, Guid invitationId) { }

        [HttpPost("{token}/accept")]
        public async Task<IActionResult> Accept(Guid orgId, string token) {
            var currentUserId = User.GetUserId();
            var result = await _invitationService.AcceptAsync(orgId, currentUserId, token);
            return Ok(result);
        }
    }
}