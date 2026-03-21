using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Data;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Models;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly AppDbContext _db;
        private readonly IMembershipService _membershipService;

        public OrganizationService(AppDbContext db, IMembershipService membershipService)
        {
            _db = db;
            _membershipService = membershipService;
        }

        public async Task<Organization> CreateAsync(CreateOrganizationRequestDTO request, Guid userId)
        {
            var user = await _db.Users.FindAsync(userId)
                ?? throw new UserNotFoundException();

            var now = DateTime.UtcNow;

            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = now
            };

            _db.Organizations.Add(organization);
            await _db.SaveChangesAsync();

            // Create owner membership
            await _membershipService.CreateAsync(user.Id, organization.Id, MembershipRole.Owner);

            return organization;
        }

        public async Task<IReadOnlyCollection<Organization>> GetAllByUserIdAsync(Guid userId)
        {
            var memberships = await _membershipService.GetByUserIdAsync(userId);

            return memberships
                .Select(x => x.Organization)
                .OrderBy(x => x.Name)
                .ToList();
        }

        public async Task<Organization?> GetByIdAsync(Guid id)
        {
            return await _db.Organizations.FindAsync(id);
        }

        public async Task<Organization> UpdateAsync(Guid id, UpdateOrganizationRequestDTO request)
        {
            var organization = await GetByIdAsync(id) ?? throw new OrganizationNotFoundException();

            if (request.Name is null && request.Description is null)
                throw new DomainException("No fields were provided for update.");

            if (request.Name is not null)
                organization.Name = request.Name;

            if (request.Description is not null)
                organization.Description = request.Description;

            organization.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return organization;
        }

        public async Task DeleteAsync(Guid id)
        {
            var organization = await GetByIdAsync(id) ?? throw new OrganizationNotFoundException();

            _db.Organizations.Remove(organization);
            await _db.SaveChangesAsync();
        }
    }
}