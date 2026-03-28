using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Constants;
using OnBoardy.API.Data;
using OnBoardy.API.DTOs;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Extensions;
using OnBoardy.API.Models;
using OnBoardy.API.Results;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IMediaStorageService _mediaStorageService;
        private readonly IMapperService _mapperService;

        public UserService(
            AppDbContext db,
            IMediaStorageService mediaBlobPipelineService,
            IMapperService mapperService)
        {
            _db = db;
            _mediaStorageService = mediaBlobPipelineService;
            _mapperService = mapperService;
        }

        public async Task<Result<User>> CreateAsync(RegisterRequest registerRequest)
        {
            if (await EmailExistsAsync(registerRequest.Email))
                return Result.Failure<User>(UserErrors.EmailAlreadyRegistered);

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

            return Result.Success(user);
        }

        public async Task<Result<User>> GetByEmailAsync(string email)
        {
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);

            return user is null
                ? Result.Failure<User>(UserErrors.NotFound)
                : Result.Success(user);
        }

        public async Task<Result<User>> GetByIdAsync(Guid id)
        {
            var user = await _db.Users.FindAsync(id);

            return user is null
                ? Result.Failure<User>(UserErrors.NotFound)
                : Result.Success(user);
        }

        public async Task<Result<User>> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var userResult = await GetByIdAsync(id);
            if (userResult.IsFailure)
                return Result.Failure<User>(userResult.Error);

            if (request.FirstName is null &&
                request.LastName is null &&
                request.IsActive is null)
            {
                return Result.Failure<User>(UserErrors.EmptyUpdatePayload);
            }

            var user = userResult.Value;

            if (request.FirstName is not null)
                user.FirstName = request.FirstName;

            if (request.LastName is not null)
                user.LastName = request.LastName;

            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Result.Success(user);
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user is null)
                return Result.Failure(UserErrors.NotFound);

            _db.Users.Remove(user.Value);
            await _db.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> VerifyEmailAsync(Guid userId)
        {
            var userResult = await GetByIdAsync(userId);
            if (userResult.IsFailure)
                return Result.Failure(userResult.Error);

            var user = userResult.Value;

            if (user.EmailVerified)
                return Result.Failure(UserErrors.EmailAlreadyVerified);

            user.EmailVerified = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _db.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<Result> SaveProfilePictureAsync(
            Guid userId,
            Stream content,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user is null)
                return Result.Failure(UserErrors.NotFound);

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var normalizedContentType = contentType.ToLowerInvariant();
            var blobName = $"{userId:N}/{Guid.NewGuid():N}{extension}";

            await _mediaStorageService.UploadAndReplaceAsync(
                BlobContainers.ProfilePictures,
                blobName,
                content,
                normalizedContentType,
                user.ProfilePictureBlobName,
                cancellationToken);

            user.ProfilePictureBlobName = blobName;
            user.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<UserResponse> GetMeAsync(Guid userId)
        {
            var user = await _db.Users.FindAsync(userId)
                ?? throw new UserNotFoundException();

            return _mapperService.ToUserResponse(user);
        }
    }
}