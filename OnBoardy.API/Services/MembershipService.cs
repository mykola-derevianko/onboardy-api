using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Data;
using OnBoardy.API.Enums;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Models;
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

        public async Task<Membership> CreateAsync(Guid userId, Guid organizationId, MembershipRole role = MembershipRole.Employee)
        {
            var user = await _db.Users.FindAsync(userId)
                ?? throw new UserNotFoundException();

            var organization = await _db.Organizations.FindAsync(organizationId)
                ?? throw new OrganizationNotFoundException();

            var alreadyMember = await _db.Memberships
                .AnyAsync(x => x.UserId == userId && x.OrganizationId == organizationId);

            if (alreadyMember)
                throw new DomainException("User is already a member of this organization.");

            var now = DateTime.UtcNow;

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                OrganizationId = organization.Id,
                Organization = organization,
                Role = role,
                Status = MembershipStatus.Active,
                CreatedAt = now
            };

            _db.Memberships.Add(membership);
            await _db.SaveChangesAsync();

            return membership;
        }

        public async Task<IReadOnlyCollection<Membership>> GetAllByOrganizationIdAsync(Guid organizationId)
        {
            return await _db.Memberships
                .AsNoTracking()
                .Where(x => x.OrganizationId == organizationId && x.Status == MembershipStatus.Active)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Membership>> GetAllByUserIdAsync(Guid userId)
        {
            return await _db.Memberships
                .AsNoTracking()
                .Include(x => x.Organization)
                .Where(x => x.UserId == userId && x.Status == MembershipStatus.Active)
                .OrderBy(x => x.Organization.Name)
                .ToListAsync();
        }

        public async Task<Membership?> GetByIdAsync(Guid id)
        {
            return await _db.Memberships.FindAsync(id);
        }

        public async Task<Membership> UpdateRoleAsync(Guid membershipId, MembershipRole role)
        {
            var membership = await GetByIdAsync(membershipId)
                ?? throw new DomainException("Membership not found.");

            membership.Role = role;
            await _db.SaveChangesAsync();

            return membership;
        }

        public async Task RemoveAsync(Guid membershipId)
        {
            var membership = await GetByIdAsync(membershipId)
                ?? throw new DomainException("Membership not found.");

            _db.Memberships.Remove(membership);
            await _db.SaveChangesAsync();
        }
    }
}