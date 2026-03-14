using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Data;
using OnBoardy.API.DTOs;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Exceptions.Identity;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IEmailVerificationService _emailVerification;
        private readonly AppDbContext _db;

        public AuthService(
            IUserService userService,
            ITokenService tokenService,
            IEmailVerificationService emailVerification,
            AppDbContext db)
        {
            _userService = userService;
            _tokenService = tokenService;
            _emailVerification = emailVerification;
            _db = db;
        }

        public async Task RegisterAsync(RegisterRequestDTO request)
        {
            var user = await _userService.CreateAsync(request);
            await _emailVerification.SendVerificationEmailAsync(user);
        }

        public async Task<TokenDTO> LoginAsync(LoginRequestDTO request, string ip)
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

            return new TokenDTO { AccessToken = access, RefreshToken = refresh.Token };
        }

        public async Task<TokenDTO> RefreshAsync(string refreshToken, string ip)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new InvalidRefreshTokenException();

            var token = await _db.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == refreshToken)
                ?? throw new InvalidRefreshTokenException();

            if (token.RevokedAt != null)
                throw new InvalidRefreshTokenException();

            if (token.ExpiresAt < DateTime.UtcNow)
                throw new TokenExpiredException();

            token.RevokedAt = DateTime.UtcNow;

            var access = _tokenService.CreateAccessToken(token.User!);
            var newRefresh = await _tokenService.CreateRefreshTokenAsync(token.User!.Id, ip);

            await _db.SaveChangesAsync();
            return new TokenDTO { AccessToken = access, RefreshToken = newRefresh.Token };
        }

        public async Task VerifyEmailAsync(string token)
        {
            var record = await _emailVerification.ValidateTokenAsync(token);
            await _emailVerification.MarkAsVerifiedAsync(record);
        }

        public async Task LogoutAsync(string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            var token = await _db.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (token is null || token.RevokedAt != null)
                return;

            token.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
}
