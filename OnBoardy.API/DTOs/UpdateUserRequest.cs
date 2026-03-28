using System.ComponentModel.DataAnnotations;

namespace OnBoardy.API.DTOs
{
    public record UpdateUserRequest
    {
        [MinLength(2)]
        public string? FirstName { get; init; }

        [MinLength(2)]
        public string? LastName { get; init; }

        public bool? IsActive { get; init; }
    }
}