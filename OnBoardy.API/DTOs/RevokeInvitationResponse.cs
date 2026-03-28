namespace OnBoardy.API.DTOs
{
    public record RevokeInvitationResponse
    {
        public required string Message { get; init; }
        public required InvitationResponse Invitation { get; init; }
    }
}