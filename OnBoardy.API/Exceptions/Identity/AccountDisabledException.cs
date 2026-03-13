using System.Net;

namespace OnBoardy.API.Exceptions.Identity
{
    public class AccountDisabledException : IdentityException
    {
        public AccountDisabledException(string message = "Account disabled")
            : base(message, HttpStatusCode.Forbidden)
        {
        }
    }
}