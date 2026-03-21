using Microsoft.AspNetCore.Mvc.Filters;
using OnBoardy.API.Enums;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Exceptions.Identity;
using OnBoardy.API.Services.Infrastructure;
using System.Security.Claims;

public class AuthorizeRoleFilter : IAsyncActionFilter
{
    private readonly MembershipRole[] _allowedRoles;
    private readonly IMembershipService _membershipService;

    public AuthorizeRoleFilter(MembershipRole[] allowedRoles, IMembershipService membershipService)
    {
        _allowedRoles = allowedRoles;
        _membershipService = membershipService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!Guid.TryParse(context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out var userId))
        {
            throw new InvalidUserContextException();
        }

        if (!context.ActionArguments.TryGetValue("id", out var idObj) || idObj is not Guid organizationId)
            throw new InvalidOrganizationContextException();

        var memberships = await _membershipService.GetByUserIdAsync(userId);

        var hasRole = memberships.Any(m =>
            m.OrganizationId == organizationId &&
            _allowedRoles.Contains(m.Role));

        if (!hasRole)
            throw new InsufficientPermissionsException();

        await next();
    }
}