using System.Net;

namespace OnBoardy.API.Exceptions.Identity
{
    public class InvalidEmailVerificationTokenException : IdentityException
    {
        public InvalidEmailVerificationTokenException(string message = "Invalid or expired email verification token")
            : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
}