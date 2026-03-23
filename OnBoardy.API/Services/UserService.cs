using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Constants;
using OnBoardy.API.Data;
using OnBoardy.API.DTOs;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Models;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IBlobService _blobService;

        public UserService(AppDbContext db, IBlobService blobService)
        {
            _db = db;
            _blobService = blobService;
        }

        public async Task<User> CreateAsync(RegisterRequest registerRequest)
        {
            if (await EmailExistsAsync(registerRequest.Email))
                throw new EmailAlreadyRegisteredException();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = registerRequest.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                EmailVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task<User> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var user = await GetByIdAsync(id) ?? throw new UserNotFoundException();

            if (request.FirstName is null &&
                request.LastName is null &&
                request.IsActive is null)
            {
                throw new DomainException("No fields were provided for update.");
            }

            if (request.FirstName is not null)
                user.FirstName = request.FirstName;

            if (request.LastName is not null)
                user.LastName = request.LastName;

            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id) ?? throw new UserNotFoundException();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task VerifyEmailAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId)
                ?? throw new UserNotFoundException();

            user.EmailVerified = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _db.Users.AnyAsync(x => x.Email == email);
        }

        public async Task SaveProfilePictureAsync(
            Guid userId,
            Stream content,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            var user = await _db.Users.FindAsync(userId) ?? throw new UserNotFoundException();

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(extension))
                throw new DomainException("Invalid file type");

            var blobName = $"{userId:N}/{Guid.NewGuid():N}{extension}";

            await _blobService.UploadAsync(
                BlobContainers.ProfilePictures,
                blobName,
                content,
                contentType,
                cancellationToken);

            if (!string.IsNullOrEmpty(user.ProfilePictureBlobName))
            {
                await _blobService.DeleteAsync(
                    BlobContainers.ProfilePictures,
                    user.ProfilePictureBlobName);
            }

            user.ProfilePictureBlobName = blobName;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }
    }
}