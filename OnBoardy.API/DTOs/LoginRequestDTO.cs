using System.ComponentModel.DataAnnotations;

namespace OnBoardy.API.DTOs
{
    public record LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = default!;

        [Required]
        public string Password { get; init; } = default!;
    }
}
