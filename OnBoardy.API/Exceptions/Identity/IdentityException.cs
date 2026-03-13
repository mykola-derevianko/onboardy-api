using System.Net;

namespace OnBoardy.API.Exceptions.Identity
{
    public class IdentityException : BaseException
    {
        public IdentityException(string message, HttpStatusCode statusCode = HttpStatusCode.Unauthorized) 
            : base(message, statusCode)
        {}
    }
}
