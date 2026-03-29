using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Data;
using OnBoardy.API.Models;
using OnBoardy.API.Results;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _email;
        private readonly IConfiguration _config;

        public EmailVerificationService(AppDbContext db, IEmailService email, IConfiguration config)
        {
            _db = db;
            _email = email;
            _config = config;
        }

        public async Task SendVerificationEmailAsync(User user)
        {
            var token = Guid.NewGuid().ToString();

            _db.EmailVerification.Add(new EmailVerification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            });

            await _db.SaveChangesAsync();

            var baseUrl = _config["App:ClientUrl"] ?? "http://localhost:3000";
            var link = $"{baseUrl}/verify-email?token={token}";

            await _email.SendAsync(
                user.Email,
                "Verify Email",
                $"Click <a href='{link}'>here</a> to verify your email"
            );
        }

        public async Task<Result<EmailVerification>> ValidateTokenAsync(string token)
        {
            var record = await _db.EmailVerification
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == token);

            if (record is null)
                return Result.Failure<EmailVerification>(AuthErrors.InvalidEmailVerificationToken);

            if (record.VerifiedAt != null)
                return Result.Failure<EmailVerification>(AuthErrors.InvalidEmailVerificationToken);

            if (record.ExpiresAt < DateTime.UtcNow)
                return Result.Failure<EmailVerification>(AuthErrors.InvalidEmailVerificationToken);

            return Result.Success(record);
        }

        public async Task<Result> MarkAsVerifiedAsync(EmailVerification record)
        {
            record.VerifiedAt = DateTime.UtcNow;
            record.User.EmailVerified = true;
            await _db.SaveChangesAsync();

            return Result.Success();
        }
    }
}