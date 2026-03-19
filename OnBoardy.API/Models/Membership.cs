using OnBoardy.API.Enums;
namespace OnBoardy.API.Models
{
    public class Membership
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public required User User { get; set; }

        public Guid OrganizationId { get; set; }
        public required Organization Organization { get; set; }

        public MembershipRole Role { get; set; }
        public MembershipStatus Status { get; set; }

        public DateTime? JoinedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
