using System.Net;

namespace OnBoardy.API.Exceptions.Domain
{
    public class DomainException : BaseException
    {
        public DomainException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) 
            : base(message, statusCode)
        {   

        }
    }
}
