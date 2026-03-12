using System.ComponentModel.DataAnnotations;

namespace OnBoardy.API.DTOs
{
    public record RegisterRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = default!;

        [Required]
        [MinLength(8)]
        public string Password { get; init; } = default!;

        [Required]
        public string FirstName { get; init; } = default!;

        [Required]
        public string LastName { get; init; } = default!;
    }
}
