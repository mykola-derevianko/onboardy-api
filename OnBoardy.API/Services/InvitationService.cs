using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Data;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Models;
using OnBoardy.API.Services.Infrastructure;
using System.Security.Cryptography;
using System.Text;

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

            //Should be handled on model validation level?
            if (request.Role == MembershipRole.Owner)
                throw new DomainException("Owner role cannot be assigned through invitation.");

            var expiresAt = request.ExpiresAt ?? DateTime.UtcNow.AddDays(7);

            const int maxRetries = 5;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                var token = GenerateSecureString(8);

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

                try
                {
                    await _db.SaveChangesAsync();
                    return invitation;
                }
                catch (UniqueConstraintException ex) {
                    _db.Entry(invitation).State = EntityState.Detached;
                    if (attempt == maxRetries - 1)
                        throw new InvitationTokenGenerationFailedException();
                }
            }

            return null;
        }

        public async Task<bool> AcceptAsync(Guid userId, string token)
        {
            var invitation = await _db.Invitations
                .FirstOrDefaultAsync(i => i.Token == token);

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

            await _membershipService.CreateAsync(userId, invitation.OrganizationId, invitation.Role);

            //
            //TODO: Handle assigned modules
            //

            invitation.Status = InvitationStatus.Accepted;
            invitation.Email = user.Email;
            invitation.AcceptedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task DeleteAsync(Guid orgId, Guid invitationId)
        {
            var invitation = await _db.Invitations
                .FirstOrDefaultAsync(i => i.Id == invitationId && i.OrganizationId == orgId);

            if (invitation == null)
                throw new InvalidInvitationException("Invitation not found.");

            if (invitation.Status != InvitationStatus.Pending)
                throw new InvalidInvitationException("Only pending invitations can be deleted.");

            _db.Invitations.Remove(invitation);
            await _db.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Invitation>> GetByOrganizationAsync(Guid orgId)
        {
            var organization = await _organizationService.GetByIdAsync(orgId)
                ?? throw new OrganizationNotFoundException();

            var invitations = await _db.Invitations
                .AsNoTracking()
                .Where(i => i.OrganizationId == orgId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return invitations;
        }

        private static string GenerateSecureString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                result.Append(chars[RandomNumberGenerator.GetInt32(chars.Length)]);
            }

            return result.ToString();
        }
    }
}
