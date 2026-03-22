using System.ComponentModel.DataAnnotations;

namespace OnBoardy.API.DTOs
{
    public record CreateOrganizationRequest
    {
        [Required]
        [MinLength(2)]
        [MaxLength(255)]
        public required string Name { get; init; }

        [MaxLength(1000)]
        public string? Description { get; init; }
    }
}