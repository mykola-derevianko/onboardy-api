using System.ComponentModel.DataAnnotations;
using OnBoardy.API.Enums;

namespace OnBoardy.API.DTOs
{
    public record CreateInvitationRequest
    {
        [EmailAddress]
        public string? Email { get; init; }

        [Required]
        public required MembershipRole Role { get; init; }

        public List<Guid>? AssignedModules { get; init; }

        public DateTime? ExpiresAt { get; init; }
    }
}