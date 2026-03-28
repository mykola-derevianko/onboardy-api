using OnBoardy.API.DTOs;
using OnBoardy.API.Models;
using OnBoardy.API.Results;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IModuleService
    {
        Task<Result<Module>> CreateAsync(CreateModuleRequest request, Guid orgId, Guid creatorUserId);
        Task<Result<Module>> GetByIdAsync(Guid moduleId);
        Task<Result<Module>> UpdateAsync(Guid moduleId, UpdateModuleRequest request, Guid updaterUserId);
        Task<Result> DeleteAsync(Guid id);

        Task<Result> SaveBannerAsync(
            Guid moduleId,
            Stream content,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default);
    }
}