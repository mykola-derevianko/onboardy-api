using System.ComponentModel.DataAnnotations;

namespace OnBoardy.API.DTOs
{
    public record RegisterRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        [MinLength(8)]
        public required string Password { get; init; }

        [Required]
        [MinLength(2)]
        public required string FirstName { get; init; }

        [Required]
        [MinLength(2)]
        public required string LastName { get; init; }
    }
}
