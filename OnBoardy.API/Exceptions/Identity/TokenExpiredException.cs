using System.Net;

namespace OnBoardy.API.Exceptions.Identity
{
    public class TokenExpiredException : IdentityException
    {
        public TokenExpiredException(string message = "Token expired")
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }
}