namespace OnBoardy.API.Models;

public class Department
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public Guid LastUpdatedBy { get; set; }

    public required Organization Organization { get; set; }
    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();
}