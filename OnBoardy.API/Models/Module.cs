using OnBoardy.API.Enums;

namespace OnBoardy.API.Models
{
    public class Module
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public string? BannerBlobName { get; set; }

        public ModuleStatus Status { get; set; } = ModuleStatus.Draft;

        public Guid OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public Guid CreatedBy { get; set; }
        public User? CreatedByUser { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
