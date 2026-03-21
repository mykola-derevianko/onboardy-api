using OnBoardy.API.Enums;

namespace OnBoardy.API.DTOs
{
    public record MembershipResponseDTO
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; init; }
        public required Guid OrganizationId { get; init; }
        public required MembershipRole Role { get; init; }
        public required MembershipStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}