using System.ComponentModel.DataAnnotations;

namespace OnBoardy.API.DTOs
{
    public record LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        [MinLength(8)]
        public required string Password { get; init; }
    }
}
