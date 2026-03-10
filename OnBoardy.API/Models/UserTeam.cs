namespace OnBoardy.API.Models
{
    public class UserTeam
    {
        public Guid UserId { get; set; }
        public Guid TeamId { get; set; }
        
        public required User User { get; set; }
        public required Team Team { get; set; }

    }
}
