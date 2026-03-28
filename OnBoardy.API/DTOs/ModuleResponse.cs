using OnBoardy.API.Enums;

namespace OnBoardy.API.DTOs
{
    public record ModuleResponse
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
        public ModuleStatus Status { get; init; }
        public Guid OrganizationId { get; init; }
        public Guid CreatedBy { get; init; }
        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public string? BannerBlobUrl { get; init; }
    }
}