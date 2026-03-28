using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Data;
using OnBoardy.API.Enums;
using OnBoardy.API.Models;
using OnBoardy.API.Results;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly AppDbContext _db;

        public MembershipService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Result<Membership>> CreateAsync(Guid userId, Guid organizationId, MembershipRole role = MembershipRole.Employee)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user is null)
                return Result.Failure<Membership>(UserErrors.NotFound);

            var organization = await _db.Organizations.FindAsync(organizationId);
            if (organization is null)
                return Result.Failure<Membership>(OrganizationErrors.NotFound);

            var alreadyMember = await _db.Memberships
                .AnyAsync(x => x.UserId == userId && x.OrganizationId == organizationId);

            if (alreadyMember)
                return Result.Failure<Membership>(MembershipErrors.AlreadyMember);

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                OrganizationId = organization.Id,
                Organization = organization,
                Role = role,
                Status = MembershipStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _db.Memberships.Add(membership);
            await _db.SaveChangesAsync();

            return Result.Success(membership);
        }

        public async Task<Result<IReadOnlyCollection<Membership>>> GetAllByOrganizationIdAsync(Guid organizationId)
        {
            var memberships = await _db.Memberships
                .AsNoTracking()
                .Where(x => x.OrganizationId == organizationId && x.Status == MembershipStatus.Active)
                .ToListAsync();

            return Result.Success<IReadOnlyCollection<Membership>>(memberships);
        }

        public async Task<Result<IReadOnlyCollection<Membership>>> GetAllByUserIdAsync(Guid userId)
        {
            var memberships = await _db.Memberships
                .AsNoTracking()
                .Include(x => x.Organization)
                .Where(x => x.UserId == userId && x.Status == MembershipStatus.Active)
                .OrderBy(x => x.Organization.Name)
                .ToListAsync();

            return Result.Success<IReadOnlyCollection<Membership>>(memberships);
        }

        public async Task<Result<Membership>> GetByIdAsync(Guid id)
        {
            var membership = await _db.Memberships.FindAsync(id);

            return membership is null
                ? Result.Failure<Membership>(MembershipErrors.NotFound)
                : Result.Success(membership);
        }

        public async Task<Result<Membership>> UpdateRoleAsync(Guid membershipId, MembershipRole role)
        {
            var membershipResult = await GetByIdAsync(membershipId);
            if (membershipResult.IsFailure)
                return Result.Failure<Membership>(membershipResult.Error);

            var membership = membershipResult.Value;
            membership.Role = role;

            await _db.SaveChangesAsync();
            return Result.Success(membership);
        }

        public async Task<Result> RemoveAsync(Guid membershipId)
        {
            var membershipResult = await GetByIdAsync(membershipId);
            if (membershipResult.IsFailure)
                return Result.Failure(membershipResult.Error);

            _db.Memberships.Remove(membershipResult.Value);
            await _db.SaveChangesAsync();

            return Result.Success();
        }
    }
}