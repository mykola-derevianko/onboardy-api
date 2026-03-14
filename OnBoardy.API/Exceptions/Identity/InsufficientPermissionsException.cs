using System.Net;

namespace OnBoardy.API.Exceptions.Identity
{
    public class InsufficientPermissionsException : IdentityException
    {
        public InsufficientPermissionsException(string message = "You do not have permission to access this resource")
            : base(message, HttpStatusCode.Forbidden)
        {
        }
    }
}