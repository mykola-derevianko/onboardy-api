using OnBoardy.API.Enums;

namespace OnBoardy.API.Models
{
    public class Invitation
    {
        public Guid Id { get; set; }

        public string? Email { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public Guid? InvitedBy { get; set; }
        public User? InvitedByUser { get; set; }

        public MembershipRole Role { get; set; }

        public List<Guid>? AssignedModules { get; set; }

        public InvitationStatus Status { get; set; }

        public required string Token { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}