using System.Net;

namespace OnBoardy.API.Exceptions.Domain
{
    public class InvalidInvitationException : DomainException
    {
        public InvalidInvitationException(string message = "Invitation is invalid, expired, or already processed")
            : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
}