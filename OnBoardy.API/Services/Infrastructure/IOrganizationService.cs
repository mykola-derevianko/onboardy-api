using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Models;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IOrganizationService
    {
        Task<Organization> CreateAsync(CreateOrganizationRequest request, Guid userId);

        Task<IReadOnlyCollection<Organization>> GetAllByUserIdAsync(Guid userId, MembershipRole? membershipRole = null);

        Task<Organization?> GetByIdAsync(Guid id);

        Task<Organization> UpdateAsync(Guid id, UpdateOrganizationRequest request);

        Task DeleteAsync(Guid id);
    }
}