namespace OnBoardy.API.Models;

public class Organization
{
    public Guid Id { get; set; }

    public Guid? OwnerId { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public User? Owner { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Department> Departments { get; set; } = new List<Department>();
}