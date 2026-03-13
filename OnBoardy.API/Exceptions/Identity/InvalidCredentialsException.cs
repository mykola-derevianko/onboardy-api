using System.Net;

namespace OnBoardy.API.Exceptions.Identity
{
    public class InvalidCredentialsException : IdentityException
    {
        public InvalidCredentialsException(string message = "Invalid credentials")
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }
}