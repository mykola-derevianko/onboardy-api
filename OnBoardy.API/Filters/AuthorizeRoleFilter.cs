using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnBoardy.API.Enums;
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
        if (!Guid.TryParse(context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
        {
            context.Result = BuildProblem(
                context,
                StatusCodes.Status401Unauthorized,
                "identity.invalid_user_context",
                "Invalid user context.");
            return;
        }

        if (!context.ActionArguments.TryGetValue("orgId", out var idObj) || idObj is not Guid organizationId)
        {
            context.Result = BuildProblem(
                context,
                StatusCodes.Status400BadRequest,
                "domain.invalid_organization_context",
                "Invalid organization context.");
            return;
        }

        var membershipsResult = await _membershipService.GetAllByUserIdAsync(userId);

        if (membershipsResult.IsFailure)
        {
            context.Result = BuildProblem(
                context,
                membershipsResult.Error.StatusCode,
                membershipsResult.Error.Code,
                membershipsResult.Error.Message);
            return;
        }

        var hasRole = membershipsResult.Value.Any(m =>
            m.OrganizationId == organizationId &&
            _allowedRoles.Contains(m.Role));

        if (!hasRole)
        {
            context.Result = BuildProblem(
                context,
                StatusCodes.Status403Forbidden,
                "identity.insufficient_permissions",
                "Insufficient permissions.");
            return;
        }

        await next();
    }

    private static ObjectResult BuildProblem(
        ActionExecutingContext context,
        int statusCode,
        string code,
        string message)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = code,
            Detail = message,
            Instance = context.HttpContext.Request.Path
        };

        return new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
    }
}