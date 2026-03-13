using OnBoardy.API.Data;
using OnBoardy.API.Models;
using OnBoardy.API.DTOs;
using OnBoardy.API.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Exceptions.Domain;

namespace OnBoardy.API.Services
{

    public class UserService : IUserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User> CreateAsync(RegisterRequestDTO registerRequest)
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
            return await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task VerifyEmailAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId)
                ?? throw new UserNotFoundException();

            user.EmailVerified = true;

            await _db.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _db.Users.AnyAsync(x => x.Email == email);
        }
    }
}
