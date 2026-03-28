using OnBoardy.API.DTOs;
namespace OnBoardy.API.Services.Infrastructure
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request);
        Task<TokenDTO> LoginAsync(LoginRequest request, string ipAddress);
        Task<TokenDTO> RefreshAsync(string refreshToken, string ipAddress);
        Task VerifyEmailAsync(string token);
        Task LogoutAsync(string? refreshToken);
    }
}
