using System.ComponentModel.DataAnnotations;

namespace OnBoardy.API.DTOs
{
    public record TokenDTO
    {
        public required string AccessToken { get; init; }
        public required string RefreshToken { get; init; }
    }
}
