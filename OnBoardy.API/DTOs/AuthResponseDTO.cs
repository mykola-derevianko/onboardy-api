using System.ComponentModel.DataAnnotations;

namespace OnBoardy.API.DTOs
{
    public record AuthResponseDTO
    {
        public required string AccessToken { get; init; }
        public required string RefreshToken { get; init; }
    }
}
