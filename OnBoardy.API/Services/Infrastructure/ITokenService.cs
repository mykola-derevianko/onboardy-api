using OnBoardy.API.Models;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);

        Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, string ipAddress);

        Task<bool> ValidateRefreshToken(string token);
    }
}
