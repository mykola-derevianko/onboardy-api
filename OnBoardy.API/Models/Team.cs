namespace OnBoardy.API.Models;

public class Team
{
    public Guid Id { get; set; }

    public Guid DepartmentId { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public Guid LastUpdatedBy { get; set; }

    public required Department Department { get; set; }
    public ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
}