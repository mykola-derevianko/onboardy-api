using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Data;
using OnBoardy.API.DTOs;
using OnBoardy.API.Exceptions.Identity;
using OnBoardy.API.Results;
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
            ICookieService cookieService,
            AppDbContext db)
        {
            _userService = userService;
            _tokenService = tokenService;
            _emailVerification = emailVerification;
            _db = db;
        }

        public async Task<Result> RegisterAsync(RegisterRequest request)
        {
            var createResult = await _userService.CreateAsync(request);
            if (createResult.IsFailure)
                return Result.Failure(createResult.Error);

            await _emailVerification.SendVerificationEmailAsync(createResult.Value);
            return Result.Success();
        }

        public async Task<Result<TokenDTO>> LoginAsync(LoginRequest request, string ip)
        {
            var userResult = await _userService.GetByEmailAsync(request.Email);
            if (userResult.IsFailure)
                return Result.Failure<TokenDTO>(AuthErrors.InvalidCredentials);

            var user = userResult.Value;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Result.Failure<TokenDTO>(AuthErrors.InvalidCredentials);

            if (!user.EmailVerified)
                return Result.Failure<TokenDTO>(AuthErrors.EmailNotVerified);

            if (!user.IsActive)
                return Result.Failure<TokenDTO>(AuthErrors.AccountDisabled);

            var access = _tokenService.CreateAccessToken(user);
            var refresh = await _tokenService.CreateRefreshTokenAsync(user.Id, ip);

            return Result.Success(new TokenDTO
            {
                AccessToken = access,
                RefreshToken = refresh.Token
            });
        }

        public async Task<Result<TokenDTO>> RefreshAsync(string refreshToken, string ip)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Result.Failure<TokenDTO>(AuthErrors.InvalidRefreshToken);

            var token = await _db.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (token is null || token.RevokedAt is not null)
                return Result.Failure<TokenDTO>(AuthErrors.InvalidRefreshToken);

            if (token.ExpiresAt < DateTime.UtcNow)
                return Result.Failure<TokenDTO>(AuthErrors.RefreshTokenExpired);

            token.RevokedAt = DateTime.UtcNow;

            var access = _tokenService.CreateAccessToken(token.User!);
            var newRefresh = await _tokenService.CreateRefreshTokenAsync(token.User!.Id, ip);

            await _db.SaveChangesAsync();

            return Result.Success(new TokenDTO
            {
                AccessToken = access,
                RefreshToken = newRefresh.Token
            });
        }

        public async Task<Result> VerifyEmailAsync(string token)
        {
            try
            {
                var record = await _emailVerification.ValidateTokenAsync(token);
                await _emailVerification.MarkAsVerifiedAsync(record);
                return Result.Success();
            }
            catch (InvalidEmailVerificationTokenException)
            {
                return Result.Failure(AuthErrors.InvalidEmailVerificationToken);
            }
            catch (TokenExpiredException)
            {
                return Result.Failure(AuthErrors.RefreshTokenExpired);
            }
        }

        public async Task<Result> LogoutAsync(string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Result.Success();

            var token = await _db.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (token is null || token.RevokedAt is not null)
                return Result.Success();

            token.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Result.Success();
        }
    }
}
