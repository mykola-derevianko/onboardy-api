using OnBoardy.API.DTOs;
namespace OnBoardy.API.Services.Infrastructure
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequestDTO request);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request, string ipAddress);
        Task<AuthResponseDTO> RefreshAsync(string refreshToken, string ipAddress);
        Task VerifyEmailAsync(string token);
    }
}
