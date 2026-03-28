using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Constants;
using OnBoardy.API.Data;
using OnBoardy.API.DTOs;
using OnBoardy.API.Models;
using OnBoardy.API.Results;
using OnBoardy.API.Services.Infrastructure;

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

        public async Task<Result<Module>> CreateAsync(CreateModuleRequest request, Guid orgId, Guid creatorUserId)
        {
            var organizationExists = await _db.Organizations.AnyAsync(o => o.Id == orgId);
            if (!organizationExists)
                return Result.Failure<Module>(ModuleErrors.OrganizationNotFound);

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

            return Result.Success(module);
        }

        public async Task<Result<Module>> UpdateAsync(Guid moduleId, UpdateModuleRequest request, Guid updaterUserId)
        {
            _ = updaterUserId;

            var module = await _db.Modules.FindAsync(moduleId);
            if (module is null)
                return Result.Failure<Module>(ModuleErrors.NotFound);

            if (request.Name is null && request.Description is null && request.Status is null)
                return Result.Failure<Module>(ModuleErrors.EmptyUpdatePayload);

            if (request.Name is not null)
                module.Name = request.Name;

            if (request.Description is not null)
                module.Description = request.Description;

            if (request.Status.HasValue)
                module.Status = request.Status.Value;

            module.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Result.Success(module);
        }

        public async Task<Result> SaveBannerAsync(
            Guid moduleId,
            Stream content,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            var module = await _db.Modules.FindAsync(moduleId);
            if (module is null)
                return Result.Failure(ModuleErrors.NotFound);

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
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var module = await _db.Modules.FindAsync(id);
            if (module is null)
                return Result.Failure(ModuleErrors.NotFound);

            _db.Modules.Remove(module);
            await _db.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<Module>> GetByIdAsync(Guid moduleId)
        {
            var module = await _db.Modules
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == moduleId);

            return module is null
                ? Result.Failure<Module>(ModuleErrors.NotFound)
                : Result.Success(module);
        }
    }
}
