using OnBoardy.API.Exceptions.Identity;
using System.Security.Claims;

namespace OnBoardy.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        extension(ClaimsPrincipal claimsPrincipal)
        {
            public Guid GetUserId()
            {
                if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));

                var claim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(claim, out var userId))
                    throw new InvalidUserContextException();

                return userId;
            }
        }
    }
}