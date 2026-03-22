namespace OnBoardy.API.DTOs
{
    public record OrganizationResponseDTO
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}