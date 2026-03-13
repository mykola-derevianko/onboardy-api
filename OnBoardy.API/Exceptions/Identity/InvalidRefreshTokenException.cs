using System.Net;

namespace OnBoardy.API.Exceptions.Identity
{
    public class InvalidRefreshTokenException : IdentityException
    {
        public InvalidRefreshTokenException(string message = "Invalid refresh token")
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }
}