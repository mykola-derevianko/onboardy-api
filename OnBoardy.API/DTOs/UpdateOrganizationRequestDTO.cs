using System.ComponentModel.DataAnnotations;

namespace OnBoardy.API.DTOs
{
    public record UpdateOrganizationRequestDTO
    {
        [MinLength(2)]
        [MaxLength(255)]
        public string? Name { get; init; }

        [MaxLength(1000)]
        public string? Description { get; init; }
    }
}