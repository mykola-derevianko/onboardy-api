using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Constants;
using OnBoardy.API.Data;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Models;
using OnBoardy.API.Results;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly AppDbContext _db;
        private readonly IMembershipService _membershipService;
        private readonly IMediaStorageService _mediaBlobPipelineService;

        public OrganizationService(
            AppDbContext db,
            IMembershipService membershipService,
            IMediaStorageService mediaBlobPipelineService)
        {
            _db = db;
            _membershipService = membershipService;
            _mediaBlobPipelineService = mediaBlobPipelineService;
        }

        public async Task<Result<Organization>> CreateAsync(CreateOrganizationRequest request, Guid userId)
        {
            var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return Result.Failure<Organization>(UserErrors.NotFound);

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

            var membershipResult = await _membershipService.CreateAsync(userId, organization.Id, MembershipRole.Owner);
            if (membershipResult.IsFailure)
                return Result.Failure<Organization>(membershipResult.Error);

            return Result.Success(organization);
        }

        public async Task<Result<IReadOnlyCollection<Organization>>> GetAllByUserIdAsync(
            Guid userId,
            IEnumerable<MembershipRole>? membershipRoles = null)
        {
            var membershipsResult = await _membershipService.GetAllByUserIdAsync(userId);
            if (membershipsResult.IsFailure)
                return Result.Failure<IReadOnlyCollection<Organization>>(membershipsResult.Error);

            IEnumerable<Membership> filteredMemberships = membershipsResult.Value;

            if (membershipRoles is not null && membershipRoles.Any())
                filteredMemberships = filteredMemberships.Where(m => membershipRoles.Contains(m.Role));

            var organizations = filteredMemberships
                .Select(m => m.Organization)
                .Distinct()
                .OrderBy(o => o.Name)
                .ToList();

            return Result.Success<IReadOnlyCollection<Organization>>(organizations);
        }

        public async Task<Result<Organization>> GetByIdAsync(Guid id)
        {
            var organization = await _db.Organizations.FindAsync(id);

            return organization is null
                ? Result.Failure<Organization>(OrganizationErrors.NotFound)
                : Result.Success(organization);
        }

        public async Task<Result<Organization>> UpdateAsync(Guid id, UpdateOrganizationRequest request)
        {
            var organizationResult = await GetByIdAsync(id);
            if (organizationResult.IsFailure)
                return Result.Failure<Organization>(organizationResult.Error);

            if (request.Name is null && request.Description is null)
                return Result.Failure<Organization>(OrganizationErrors.EmptyUpdatePayload);

            var organization = organizationResult.Value;

            if (request.Name is not null)
                organization.Name = request.Name;

            if (request.Description is not null)
                organization.Description = request.Description;

            organization.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Result.Success(organization);
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var organizationResult = await GetByIdAsync(id);
            if (organizationResult.IsFailure)
                return Result.Failure(organizationResult.Error);

            _db.Organizations.Remove(organizationResult.Value);
            await _db.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> SaveMediaAsync(
            Guid organizationId,
            Stream content,
            string fileName,
            string contentType,
            OrganizationMediaType mediaType,
            CancellationToken cancellationToken = default)
        {
            var organizationResult = await GetByIdAsync(organizationId);
            if (organizationResult.IsFailure)
                return Result.Failure(organizationResult.Error);

            var organization = organizationResult.Value;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var normalizedContentType = contentType.ToLowerInvariant();

            var mediaPath = mediaType == OrganizationMediaType.Logo ? "logo" : "banner";
            var blobName = $"organizations/{organizationId:N}/{mediaPath}/{Guid.NewGuid():N}{extension}";

            var oldBlobName = mediaType == OrganizationMediaType.Logo
                ? organization.LogoBlobName
                : organization.BannerBlobName;

            await _mediaBlobPipelineService.UploadAndReplaceAsync(
                BlobContainers.OrganizationMedia,
                blobName,
                content,
                normalizedContentType,
                oldBlobName,
                cancellationToken);

            if (mediaType == OrganizationMediaType.Logo)
                organization.LogoBlobName = blobName;
            else
                organization.BannerBlobName = blobName;

            organization.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}