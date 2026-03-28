using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Data;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Models;
using OnBoardy.API.Results;
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

        public async Task<Result<Invitation>> CreateAsync(Guid orgId, Guid invitedByUserId, CreateInvitationRequest request)
        {
            var organizationResult = await _organizationService.GetByIdAsync(orgId);
            if (organizationResult.IsFailure)
                return Result.Failure<Invitation>(organizationResult.Error);

            var invitedByUserResult = await _userService.GetByIdAsync(invitedByUserId);
            if (invitedByUserResult.IsFailure)
                return Result.Failure<Invitation>(invitedByUserResult.Error);

            if (request.Role == MembershipRole.Owner)
                return Result.Failure<Invitation>(InvitationErrors.OwnerRoleNotAllowed);

            var expiresAt = request.ExpiresAt ?? DateTime.UtcNow.AddDays(7);
            const int maxRetries = 5;

            for (var attempt = 0; attempt < maxRetries; attempt++)
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
                    return Result.Success(invitation);
                }
                catch (UniqueConstraintException)
                {
                    _db.Entry(invitation).State = EntityState.Detached;

                    if (attempt == maxRetries - 1)
                        return Result.Failure<Invitation>(InvitationErrors.TokenGenerationFailed);
                }
            }

            return Result.Failure<Invitation>(InvitationErrors.TokenGenerationFailed);
        }

        public async Task<Result> AcceptAsync(Guid userId, string token)
        {
            var invitation = await _db.Invitations
                .FirstOrDefaultAsync(i => i.Token == token);

            var userResult = await _userService.GetByIdAsync(userId);
            if (userResult.IsFailure)
                return Result.Failure(userResult.Error);

            var user = userResult.Value;

            if (invitation is null ||
                invitation.Status != InvitationStatus.Pending ||
                invitation.ExpiresAt < DateTime.UtcNow)
            {
                return Result.Failure(InvitationErrors.InvalidInvitation);
            }

            if (invitation.Email is not null &&
                !string.Equals(invitation.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Failure(InvitationErrors.EmailMismatch);
            }

            var membershipResult = await _membershipService.CreateAsync(userId, invitation.OrganizationId, invitation.Role);
            if (membershipResult.IsFailure)
                return Result.Failure(membershipResult.Error);

            invitation.Status = InvitationStatus.Accepted;
            invitation.Email = user.Email;
            invitation.AcceptedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid orgId, Guid invitationId)
        {
            var invitation = await _db.Invitations
                .FirstOrDefaultAsync(i => i.Id == invitationId && i.OrganizationId == orgId);

            if (invitation is null)
                return Result.Failure(InvitationErrors.NotFound);

            if (invitation.Status != InvitationStatus.Pending)
                return Result.Failure(InvitationErrors.OnlyPendingCanBeDeleted);

            _db.Invitations.Remove(invitation);
            await _db.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<IReadOnlyCollection<Invitation>>> GetByOrganizationAsync(Guid orgId)
        {
            var organizationResult = await _organizationService.GetByIdAsync(orgId);
            if (organizationResult.IsFailure)
                return Result.Failure<IReadOnlyCollection<Invitation>>(organizationResult.Error);

            var invitations = await _db.Invitations
                .AsNoTracking()
                .Where(i => i.OrganizationId == orgId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return Result.Success<IReadOnlyCollection<Invitation>>(invitations);
        }

        private static string GenerateSecureString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new StringBuilder(length);

            for (var i = 0; i < length; i++)
                result.Append(chars[RandomNumberGenerator.GetInt32(chars.Length)]);

            return result.ToString();
        }
    }
}
