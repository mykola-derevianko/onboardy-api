using OnBoardy.API.DTOs;
using OnBoardy.API.Models;

namespace OnBoardy.API.Extensions
{
    public static class MembershipMappingExtensions
    {
        extension(Membership membership)
        {
            public MembershipResponseDTO ToResponseDTO()
            {
                return new MembershipResponseDTO
                {
                    Id = membership.Id,
                    UserId = membership.UserId,
                    OrganizationId = membership.OrganizationId,
                    Role = membership.Role.ToString(),
                    Status = membership.Status.ToString(),
                    CreatedAt = membership.CreatedAt
                };
            }
        }
    }
}