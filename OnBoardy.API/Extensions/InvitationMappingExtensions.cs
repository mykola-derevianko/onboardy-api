using OnBoardy.API.DTOs;
using OnBoardy.API.Models;

namespace OnBoardy.API.Extensions
{
    public static class InvitationMappingExtensions
    {
        extension(Invitation invitation)
        {
            public InvitationResponseDTO ToResponseDTO()
            {
                return new InvitationResponseDTO
                {
                    Id = invitation.Id,
                    Email = invitation.Email,
                    OrganizationId = invitation.OrganizationId,
                    InvitedBy = invitation.InvitedBy,
                    Role = invitation.Role,
                    AssignedModules = invitation.AssignedModules,
                    Status = invitation.Status,
                    Token = invitation.Token,
                    CreatedAt = invitation.CreatedAt,
                    AcceptedAt = invitation.AcceptedAt,
                    ExpiresAt = invitation.ExpiresAt
                };
            }
        }
    }
}