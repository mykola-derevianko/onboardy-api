using System.Net;

namespace OnBoardy.API.Exceptions.Identity
{
    public class EmailNotVerifiedException : IdentityException
    {
        public EmailNotVerifiedException(string message = "Email not verified")
            : base(message, HttpStatusCode.Forbidden)
        {
        }
    }
}