using OnBoardy.API.DTOs;
namespace OnBoardy.API.Services.Infrastructure
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequestDTO request);
        Task<TokenDTO> LoginAsync(LoginRequestDTO request, string ipAddress);
        Task<TokenDTO> RefreshAsync(string refreshToken, string ipAddress);
        Task VerifyEmailAsync(string token);
        Task LogoutAsync(string? refreshToken);
    }
}
