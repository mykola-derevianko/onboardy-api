namespace OnBoardy.API.Models;

public class User
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }
    public User? CreatedByUser { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? LastUpdatedBy { get; set; }
    public User? LastUpdatedByUser { get; set; }

    public required Organization Organization { get; set; }
    public ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
    public ICollection<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();
    public Organization? OwnedOrganization { get; set; }
}