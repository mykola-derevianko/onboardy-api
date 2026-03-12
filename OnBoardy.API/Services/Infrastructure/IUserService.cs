using OnBoardy.API.DTOs;
using OnBoardy.API.Models;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IUserService
    {
        Task<User> CreateAsync(RegisterRequestDTO request);

        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByIdAsync(Guid id);

        Task VerifyEmailAsync(Guid userId);

        Task<bool> EmailExistsAsync(string email);

    }
}
