namespace OnBoardy.API.Models
{
    public class Organization
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public string? Description { get; set; }

        public string? LogoBlobName { get; set; }
        public string? BannerBlobName { get; set; }

        public required DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
