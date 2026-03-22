using OnBoardy.API.Models;
using OnBoardy.API.Enums;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IMembershipService
    {
        Task<Membership> CreateAsync(Guid userId, Guid organizationId, MembershipRole role = MembershipRole.Owner);

        Task<IReadOnlyCollection<Membership>> GetAllByOrganizationIdAsync(Guid organizationId);

        Task<IReadOnlyCollection<Membership>> GetAllByUserIdAsync(Guid userId);

        Task<Membership?> GetByIdAsync(Guid id);

        Task<Membership> UpdateRoleAsync(Guid membershipId, MembershipRole role);

        Task RemoveAsync(Guid membershipId);
    }
}