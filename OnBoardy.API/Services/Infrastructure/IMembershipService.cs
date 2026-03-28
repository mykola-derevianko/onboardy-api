using OnBoardy.API.Enums;
using OnBoardy.API.Models;
using OnBoardy.API.Results;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IMembershipService
    {
        Task<Result<Membership>> CreateAsync(Guid userId, Guid organizationId, MembershipRole role = MembershipRole.Employee);

        Task<Result<IReadOnlyCollection<Membership>>> GetAllByOrganizationIdAsync(Guid organizationId);

        Task<Result<IReadOnlyCollection<Membership>>> GetAllByUserIdAsync(Guid userId);

        Task<Result<Membership>> GetByIdAsync(Guid id);

        Task<Result<Membership>> UpdateRoleAsync(Guid membershipId, MembershipRole role);

        Task<Result> RemoveAsync(Guid membershipId);
    }
}