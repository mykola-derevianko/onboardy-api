using System.ComponentModel.DataAnnotations;
using OnBoardy.API.Enums;

namespace OnBoardy.API.DTOs
{
    public record UpdateModuleRequest
    {
        [MinLength(2)]
        [MaxLength(255)]
        public string? Name { get; init; }

        [MaxLength(1000)]
        public string? Description { get; init; }

        public ModuleStatus? Status { get; init; }
    }
}