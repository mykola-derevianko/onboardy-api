namespace OnBoardy.API.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public required string Email { get; set; }

        public required string PasswordHash { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? ProfilePictureBlobName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool EmailVerified { get; set; } = false;

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<EmailVerification> EmailVerifications { get; set; } = new List<EmailVerification>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
