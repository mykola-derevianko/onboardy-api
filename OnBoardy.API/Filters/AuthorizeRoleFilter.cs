using Microsoft.AspNetCore.Mvc.Filters;
using OnBoardy.API.Enums;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Exceptions.Identity;
using OnBoardy.API.Services.Infrastructure;

public class AuthorizeRoleFilter : IAsyncActionFilter
{
    private readonly MembershipRole _requiredRole;
    private readonly IMembershipService _membershipService;

    public AuthorizeRoleFilter(MembershipRole requiredRole, IMembershipService membershipService)
    {
        _requiredRole = requiredRole;
        _membershipService = membershipService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        var subject = user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                      ?? user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(subject, out var userId))
            throw new InvalidUserContextException();

        if (!context.ActionArguments.TryGetValue("id", out var idObj) || idObj is not Guid organizationId)
            throw new InvalidOrganizationContextException();

        var memberships = await _membershipService.GetByUserIdAsync(userId);
        var hasRole = memberships.Any(m => m.OrganizationId == organizationId && m.Role >= _requiredRole);

        if (!hasRole)
            throw new InsufficientPermissionsException();

        await next();
    }
}