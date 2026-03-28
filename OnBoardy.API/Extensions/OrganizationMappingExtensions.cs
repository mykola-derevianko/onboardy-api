using OnBoardy.API.DTOs;
using OnBoardy.API.Models;

namespace OnBoardy.API.Extensions
{
    public static class OrganizationMappingExtensions
    {
        extension(Organization organization)
        {
            public OrganizationResponse ToResponseDTO()
            {
                return new OrganizationResponse
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