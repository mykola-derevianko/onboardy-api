using OnBoardy.API.Enums;

namespace OnBoardy.API.DTOs
{
    public record InvitationResponseDTO
    {
        public required Guid Id { get; init; }
        public string? Email { get; init; }
        public required Guid OrganizationId { get; init; }
        public Guid? InvitedBy { get; init; }
        public required MembershipRole Role { get; init; }
        public List<Guid>? AssignedModules { get; init; }
        public required InvitationStatus Status { get; init; }
        public required string Token { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? AcceptedAt { get; init; }
        public DateTime? ExpiresAt { get; init; }
    }
}