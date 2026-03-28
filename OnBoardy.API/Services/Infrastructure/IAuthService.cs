using OnBoardy.API.DTOs;
using OnBoardy.API.Results;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterRequest request);
        Task<Result<TokenDTO>> LoginAsync(LoginRequest request, string ipAddress);
        Task<Result<TokenDTO>> RefreshAsync(string refreshToken, string ipAddress);
        Task<Result> VerifyEmailAsync(string token);
        Task<Result> LogoutAsync(string? refreshToken);
    }
}
