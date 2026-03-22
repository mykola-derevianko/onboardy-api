using OnBoardy.API.DTOs;
using OnBoardy.API.Models;

namespace OnBoardy.API.Extensions
{
    public static class OrganizationMappingExtensions
    {
        extension(Organization organization)
        {
            public OrganizationResponseDTO ToResponseDTO()
            {
                return new OrganizationResponseDTO
                {
                    Id = organization.Id,
                    Name = organization.Name,
                    Description = organization.Description,
                    CreatedAt = organization.CreatedAt,
                    UpdatedAt = organization.UpdatedAt
                };
            }
        }
    }
}