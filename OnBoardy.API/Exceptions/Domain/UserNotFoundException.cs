using System.Net;

namespace OnBoardy.API.Exceptions.Domain
{
    public class UserNotFoundException : DomainException
    {
        public UserNotFoundException(string message = "User not found")
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }
}