using System.ComponentModel.DataAnnotations;

namespace OnBoardy.API.DTOs
{
    public record RegisterRequestDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        [MinLength(8)]
        public required string Password { get; init; }

        [Required]
        public required string FirstName { get; init; }

        [Required]
        public required string LastName { get; init; }
    }
}
