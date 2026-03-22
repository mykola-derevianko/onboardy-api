namespace OnBoardy.API.DTOs
{
    public record RevokeInvitationResponseDTO
    {
        public required string Message { get; init; }
        public required InvitationResponseDTO Invitation { get; init; }
    }
}