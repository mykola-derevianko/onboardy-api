namespace OnBoardy.API.DTOs
{
    public record UserResponseDTO
    {
        public required Guid Id { get; init; }
        public required string Email { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public bool IsActive { get; init; }
        public bool EmailVerified { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}