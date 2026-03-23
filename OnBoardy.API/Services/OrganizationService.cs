using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Constants;
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
        private static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private static readonly HashSet<string> AllowedContentTypes = ["image/jpg", "image/jpeg", "image/png", "image/webp"];

        private readonly AppDbContext _db;
        private readonly IMembershipService _membershipService;
        private readonly IBlobService _blobService;

        public OrganizationService(
            AppDbContext db,
            IMembershipService membershipService,
            IBlobService blobService)
        {
            _db = db;
            _membershipService = membershipService;
            _blobService = blobService;
        }

        public async Task<Organization> CreateAsync(CreateOrganizationRequest request, Guid userId)
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

        public async Task<IReadOnlyCollection<Organization>> GetAllByUserIdAsync(Guid userId, IEnumerable<MembershipRole>? membershipRoles = null)
        {
            var memberships = await _membershipService.GetAllByUserIdAsync(userId);

            IEnumerable<Membership> filteredMemberships = memberships;

            if (membershipRoles != null && membershipRoles.Any())
                filteredMemberships = filteredMemberships.Where(m => membershipRoles.Contains(m.Role));

            var organizations = filteredMemberships
                .Select(m => m.Organization)
                .Distinct()
                .OrderBy(o => o.Name)
                .ToList();

            return organizations;
        }

        public async Task<Organization?> GetByIdAsync(Guid id)
        {
            return await _db.Organizations.FindAsync(id);
        }

        public async Task<Organization> UpdateAsync(Guid id, UpdateOrganizationRequest request)
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

        public Task SaveLogoAsync(
            Guid organizationId,
            Stream content,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            return SaveMediaAsync(organizationId, content, fileName, contentType, isLogo: true, cancellationToken);
        }

        public Task SaveBannerAsync(
            Guid organizationId,
            Stream content,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            return SaveMediaAsync(organizationId, content, fileName, contentType, isLogo: false, cancellationToken);
        }

        private async Task SaveMediaAsync(
            Guid organizationId,
            Stream content,
            string fileName,
            string contentType,
            bool isLogo,
            CancellationToken cancellationToken)
        {
            var organization = await _db.Organizations.FindAsync(organizationId)
                ?? throw new OrganizationNotFoundException();

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new DomainException("Invalid file type.");

            var normalizedContentType = (contentType ?? string.Empty).ToLowerInvariant();
            if (!AllowedContentTypes.Contains(normalizedContentType))
                throw new DomainException("Invalid content type.");

            var mediaType = isLogo ? "logo" : "banner";
            var blobName = $"organizations/{organizationId:N}/{mediaType}/{Guid.NewGuid():N}{extension}";

            await _blobService.UploadAsync(
                BlobContainers.OrganizationMedia,
                blobName,
                content,
                normalizedContentType,
                cancellationToken);

            var oldBlobName = isLogo
                ? organization.LogoBlobName
                : organization.BannerBlobName;

            if (!string.IsNullOrWhiteSpace(oldBlobName))
            {
                await _blobService.DeleteAsync(
                    BlobContainers.OrganizationMedia,
                    oldBlobName);
            }

            if (isLogo)
                organization.LogoBlobName = blobName;
            else
                organization.BannerBlobName = blobName;

            organization.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}