using OnBoardy.API.DTOs;
using OnBoardy.API.Models;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IOrganizationService
    {
        Task<Organization> CreateAsync(CreateOrganizationRequestDTO request, Guid userId);

        Task<IReadOnlyCollection<Organization>> GetAllByUserIdAsync(Guid userId);

        Task<Organization?> GetByIdAsync(Guid id);

        Task<Organization> UpdateAsync(Guid id, UpdateOrganizationRequestDTO request);

        Task DeleteAsync(Guid id);
    }
}