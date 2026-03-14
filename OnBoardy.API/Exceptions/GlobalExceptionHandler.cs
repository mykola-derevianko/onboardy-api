using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Exceptions.Identity;

namespace OnBoardy.API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var problemDetails = exception switch
            {
                IdentityException identityEx => new ProblemDetails
                {
                    Status = (int)identityEx.StatusCode,
                    Title = "Security Error",
                    Type = "identity-error",
                    Detail = identityEx.Message
                },

                DomainException domainEx => new ProblemDetails
                {
                    Status = (int)domainEx.StatusCode,
                    Title = "Business Rule Violation",
                    Type = "domain-error",
                    Detail = domainEx.Message
                },

                BaseException baseEx => new ProblemDetails
                {
                    Status = (int)baseEx.StatusCode,
                    Title = "Application Error",
                    Detail = baseEx.Message
                },

                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Type = "server-error",
                    Detail = exception.Message
                }

            };
            problemDetails.Instance = httpContext.Request.Path;
            httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
