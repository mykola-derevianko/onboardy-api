namespace OnBoardy.API.DTOs
{
    public record OrganizationResponse
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}