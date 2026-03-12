using System.ComponentModel.DataAnnotations;

namespace OnBoardy.API.DTOs
{
    public record AuthResponseDTO
    {
        public string AccessToken { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
    }
}
