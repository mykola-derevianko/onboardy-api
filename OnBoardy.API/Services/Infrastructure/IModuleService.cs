using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Models;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IModuleService
    {
        Task<Module?> CreateAsync(CreateModuleRequest request, Guid orgId, Guid creatorUserId);
        Task<Module?> GetByIdAsync(Guid moduleId);
        Task<Module?> UpdateAsync(Guid moduleId, UpdateModuleRequest request, Guid updaterUserId);
        Task DeleteAsync(Guid id);

        Task SaveBannerAsync(
            Guid moduleId,
            Stream content,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default);
    }
}