using OnBoardy.API.DTOs;
using OnBoardy.API.Models;
using OnBoardy.API.Results;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IInvitationService
    {
        Task<Result<Invitation>> CreateAsync(Guid orgId, Guid invitedByUserId, CreateInvitationRequest request);
        Task<Result<IReadOnlyCollection<Invitation>>> GetByOrganizationAsync(Guid orgId);
        Task<Result> AcceptAsync(Guid userId, string token);
        Task<Result> DeleteAsync(Guid orgId, Guid invitationId);
    }
}