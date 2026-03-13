using System.Net;

namespace OnBoardy.API.Exceptions.Domain
{
    public class InvalidEmailVerificationTokenException : DomainException
    {
        public InvalidEmailVerificationTokenException(string message = "Invalid email verification token")
            : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
}