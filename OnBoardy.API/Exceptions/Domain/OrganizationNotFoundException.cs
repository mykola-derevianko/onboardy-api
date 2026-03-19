using System.Net;

namespace OnBoardy.API.Exceptions.Domain
{
    public class OrganizationNotFoundException : DomainException
    {
        public OrganizationNotFoundException(string message = "Organization not found")
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }
}