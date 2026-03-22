using OnBoardy.API.DTOs;
using OnBoardy.API.Models;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IInvitationService
    {
        Task<Invitation?> CreateAsync(Guid orgId, Guid invitedByUserId, CreateInvitationRequestDTO request);
        Task<IReadOnlyCollection<Invitation>> GetByOrganizationAsync(Guid orgId);
        Task<bool> AcceptAsync(Guid userId, string token);
        Task DeleteAsync(Guid orgId, Guid invitationId);
    }
}