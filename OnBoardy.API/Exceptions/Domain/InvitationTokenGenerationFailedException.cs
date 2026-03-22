using System.Net;

namespace OnBoardy.API.Exceptions.Domain
{
    public class InvitationTokenGenerationFailedException : DomainException
    {
        public InvitationTokenGenerationFailedException(
            string message = "Failed to generate a unique invitation token after multiple attempts.")
            : base(message, HttpStatusCode.InternalServerError)
        {
        }
    }
}