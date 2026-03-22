using OnBoardy.API.DTOs;
using OnBoardy.API.Models;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IUserService
    {
        Task<User> CreateAsync(RegisterRequest request);

        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByIdAsync(Guid id);

        Task<User> UpdateAsync(Guid id, UpdateUserRequest request);

        Task DeleteAsync(Guid id);

        Task VerifyEmailAsync(Guid userId);

        Task<bool> EmailExistsAsync(string email);

        UploadSasResponse GenerateProfilePictureUpload(Guid userId, string fileName);
        Task SaveProfilePictureAsync(Guid userId, string blobName);
    }
}
