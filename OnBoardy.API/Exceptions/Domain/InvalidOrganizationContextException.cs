using System.Net;

namespace OnBoardy.API.Exceptions.Domain
{
    public class InvalidOrganizationContextException : DomainException
    {
        public InvalidOrganizationContextException(
            string message = "Organization ID is missing or invalid in route")
            : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
}