using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Models;
using OnBoardy.API.Results;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IOrganizationService
    {
        Task<Result<Organization>> CreateAsync(CreateOrganizationRequest request, Guid userId);

        Task<Result<IReadOnlyCollection<Organization>>> GetAllByUserIdAsync(
            Guid userId,
            IEnumerable<MembershipRole>? membershipRoles = null);

        Task<Result<Organization>> GetByIdAsync(Guid id);

        Task<Result<Organization>> UpdateAsync(Guid id, UpdateOrganizationRequest request);

        Task<Result> DeleteAsync(Guid id);

        Task<Result> SaveMediaAsync(
            Guid organizationId,
            Stream content,
            string fileName,
            string contentType,
            OrganizationMediaType mediaType,
            CancellationToken cancellationToken = default);
    }
}