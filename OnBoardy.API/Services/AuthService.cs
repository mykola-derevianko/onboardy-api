using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Data;
using OnBoardy.API.DTOs;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Exceptions.Identity;
using OnBoardy.API.Models;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Services
{


    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _email;
        private readonly AppDbContext _db;

        public AuthService(
            IUserService userService,
            ITokenService tokenService,
            IEmailService email,
            AppDbContext db)
        {
            _userService = userService;
            _tokenService = tokenService;
            _email = email;
            _db = db;
        }

        public async Task RegisterAsync(RegisterRequestDTO request)
        {
            var user = await _userService.CreateAsync(request);

            // Create email verification token
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

            // Send verification email
            var link = $"http://localhost:3000/auth/verify-email?token={token}";
            await _email.SendAsync(
                user.Email,
                "Verify Email",
                $"Click <a href='{link}'>here</a> to verify your email"
            );
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request, string ip)
        {
            var user = await _userService.GetByEmailAsync(request.Email)
                ?? throw new UserNotFoundException();

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new InvalidCredentialsException();

            if (!user.EmailVerified)
                throw new EmailNotVerifiedException();

            if (!user.IsActive)
                throw new AccountDisabledException();

            var access = _tokenService.CreateAccessToken(user);
            var refresh = await _tokenService.CreateRefreshTokenAsync(user.Id, ip);

            return new AuthResponseDTO { AccessToken = access, RefreshToken = refresh.Token };
        }

        public async Task<AuthResponseDTO> RefreshAsync(string refreshToken, string ip)
        {
            var token = await _db.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == refreshToken)
                ?? throw new InvalidRefreshTokenException();

            if (token.RevokedAt != null)
                throw new InvalidRefreshTokenException();

            if (token.ExpiresAt < DateTime.UtcNow)
                throw new TokenExpiredException();

            token.RevokedAt = DateTime.UtcNow;

            var access = _tokenService.CreateAccessToken(token.User);
            var newRefresh = await _tokenService.CreateRefreshTokenAsync(token.User.Id, ip);

            await _db.SaveChangesAsync();
            return new AuthResponseDTO { AccessToken = access, RefreshToken = newRefresh.Token };
        }

        public async Task VerifyEmailAsync(string token)
        {
            var record = await _db.EmailVerification
                .FirstOrDefaultAsync(x => x.Token == token)
                ?? throw new InvalidEmailVerificationTokenException();

            if (record.ExpiresAt < DateTime.UtcNow)
                throw new TokenExpiredException();

            await _userService.VerifyEmailAsync(record.UserId);
        }
    }
}
