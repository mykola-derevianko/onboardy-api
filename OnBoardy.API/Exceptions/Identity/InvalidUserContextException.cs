using System.Net;

namespace OnBoardy.API.Exceptions.Identity
{
    public class InvalidUserContextException : IdentityException
    {
        public InvalidUserContextException(string message = "Invalid authenticated user context")
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }
}