using OnBoardy.API.DTOs;
using OnBoardy.API.Models;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IInvitationService
    {
        Task<Invitation?> CreateAsync(Guid orgId, Guid invitedByUserId, CreateInvitationRequestDTO request);
        Task<bool> AcceptAsync(Guid orgId, Guid userId, string token);
    }
}