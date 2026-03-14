namespace OnBoardy.API.Models
{
    public class EmailVerification
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public required string Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
