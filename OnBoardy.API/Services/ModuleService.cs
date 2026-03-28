using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Constants;
using OnBoardy.API.Data;
using OnBoardy.API.DTOs;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Models;
using OnBoardy.API.Services.Infrastructure;
using System.Net;

namespace OnBoardy.API.Services
{
    public class ModuleService : IModuleService
    {
        private readonly AppDbContext _db;
        private readonly IMediaStorageService _mediaStorageService;

        public ModuleService(AppDbContext db, IMediaStorageService mediaStorageService)
        {
            _db = db;
            _mediaStorageService = mediaStorageService;
        }

        public async Task<Module?> CreateAsync(CreateModuleRequest request, Guid orgId, Guid creatorUserId)
        {
            var organizationExists = await _db.Organizations
                .AnyAsync(o => o.Id == orgId);

            if (!organizationExists)
                throw new OrganizationNotFoundException();

            var module = new Module
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Status = request.Status,
                OrganizationId = orgId,
                CreatedBy = creatorUserId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Modules.Add(module);
            await _db.SaveChangesAsync();
            return module;
        }

        public async Task<Module?> UpdateAsync(Guid moduleId, UpdateModuleRequest request, Guid updaterUserId)
        {
            var module = await _db.Modules.FindAsync(moduleId);

            if (module == null)
                return null;

            if (request.Name is null && request.Description is null && request.Status is null)
                throw new DomainException("No fields were provided for update.");

            if (request.Name is not null)
                module.Name = request.Name;

            if (request.Description is not null)
                module.Description = request.Description;

            if (request.Status.HasValue)
                module.Status = request.Status.Value;

            module.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return module;
        }

        public async Task SaveBannerAsync(
            Guid moduleId,
            Stream content,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            var module = await _db.Modules.FindAsync(moduleId)
                ?? throw new DomainException("Module not found.", HttpStatusCode.NotFound);

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var normalizedContentType = contentType.ToLowerInvariant();

            var blobName = $"organizations/{module.OrganizationId:N}/modules/{moduleId:N}/banner/{Guid.NewGuid():N}{extension}";

            await _mediaStorageService.UploadAndReplaceAsync(
                BlobContainers.OrganizationMedia,
                blobName,
                content,
                normalizedContentType,
                module.BannerBlobName,
                cancellationToken);

            module.BannerBlobName = blobName;
            module.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id)
        {
            var module = await _db.Modules.FindAsync(id);

            if (module == null)
                throw new DomainException("Module not found.", HttpStatusCode.NotFound);

            _db.Modules.Remove(module);
            await _db.SaveChangesAsync();
        }

        public async Task<Module?> GetByIdAsync(Guid moduleId)
        {
            return await _db.Modules
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == moduleId);
        }
    }
}
