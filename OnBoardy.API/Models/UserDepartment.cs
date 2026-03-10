namespace OnBoardy.API.Models
{
    public class UserDepartment
    {
        public Guid UserId { get; set; }
        public Guid DepartmentId { get; set; }
        
        public required User User { get; set; }
        public required Department Department { get; set; }
    }
}
