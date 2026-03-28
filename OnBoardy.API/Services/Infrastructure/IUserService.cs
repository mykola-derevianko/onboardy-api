using OnBoardy.API.DTOs;
using OnBoardy.API.Models;
using OnBoardy.API.Results;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IUserService
    {
        Task<Result<User>> CreateAsync(RegisterRequest request);
        Task<Result<User>> GetByEmailAsync(string email);
        Task<Result<User>> GetByIdAsync(Guid id);
        Task<Result<User>> UpdateAsync(Guid id, UpdateUserRequest request);
        Task<Result> DeleteAsync(Guid id);
        Task<Result> VerifyEmailAsync(Guid userId);
        Task<bool> EmailExistsAsync(string email);

        Task<Result> SaveProfilePictureAsync(
            Guid userId,
            Stream content,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default);
    }
}
