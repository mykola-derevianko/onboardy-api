using OnBoardy.API.DTOs;
using OnBoardy.API.Models;

namespace OnBoardy.API.Extensions
{
    public static class UserMappingExtensions
    {
        extension(User user)
        {
            public UserResponse ToResponseDTO()
            {
                return new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    EmailVerified = user.EmailVerified,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };
            }
        }
    }
}