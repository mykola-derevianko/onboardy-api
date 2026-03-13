using System.Net;

namespace OnBoardy.API.Exceptions.Domain
{
    public class EmailAlreadyRegisteredException : DomainException
    {
        public EmailAlreadyRegisteredException(string message = "Email already registered")
            : base(message, HttpStatusCode.Conflict)
        {
        }
    }
}