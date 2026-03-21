using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Data;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Models;
using OnBoardy.API.Services.Infrastructure;
using System.Security.Cryptography;

namespace OnBoardy.API.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly AppDbContext _db;
        private readonly IOrganizationService _organizationService;
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;

        public InvitationService(
            AppDbContext db,
            IOrganizationService organizationService,
            IMembershipService membershipService,
            IUserService userService)
        {
            _db = db;
            _organizationService = organizationService;
            _membershipService = membershipService;
            _userService = userService;
        }

        public async Task<Invitation?> CreateAsync(Guid orgId, Guid invitedByUserId, CreateInvitationRequestDTO request)
        {
            var organization = await _organizationService.GetByIdAsync(orgId)
                ?? throw new OrganizationNotFoundException();

            var invitedByUser = await _userService.GetByIdAsync(invitedByUserId)
                ?? throw new UserNotFoundException();

            if (request.Role == MembershipRole.Owner)
                throw new DomainException("Owner role cannot be assigned through invitation.");

            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
            var expiresAt = request.ExpiresAt ?? DateTime.UtcNow.AddDays(7);

            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                OrganizationId = orgId,
                InvitedBy = invitedByUserId,
                Role = request.Role,
                AssignedModules = request.AssignedModules,
                Status = InvitationStatus.Pending,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            _db.Invitations.Add(invitation);
            await _db.SaveChangesAsync();

            return invitation;
        }

        public async Task<bool> AcceptAsync(Guid orgId, Guid userId, string token)
        {
            var invitation = await _db.Invitations
                .FirstOrDefaultAsync(i => i.OrganizationId == orgId && i.Token == token);

            var user = await _userService.GetByIdAsync(userId)
                ?? throw new UserNotFoundException();

            if (invitation == null ||
                invitation.Status != InvitationStatus.Pending ||
                invitation.ExpiresAt < DateTime.UtcNow)
            {
                throw new InvalidInvitationException();
            }

            if (invitation.Email is not null && !string.Equals(invitation.Email, user.Email, StringComparison.OrdinalIgnoreCase))
                throw new InvalidInvitationException();

            await _membershipService.CreateAsync(userId, orgId, invitation.Role);

            //
            //TODO: Handle assigned modules
            //

            invitation.Status = InvitationStatus.Accepted;
            invitation.Email = user.Email;
            invitation.AcceptedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
