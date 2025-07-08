using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace consware_api.Infrastructure.Authorization;

public class ResourceOwnerAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _parameterName;

    public ResourceOwnerAttribute(string parameterName = "id")
    {
        _parameterName = parameterName;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRoleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            context.Result = new ForbidResult();
            return;
        }

        if (userRoleClaim == "Aprobador")
        {
            return;
        }

        var routeValues = context.RouteData.Values;
        var actionArguments = context.ActionDescriptor.Parameters;

        string resourceUserId = null;

        if (routeValues.ContainsKey(_parameterName))
        {
            resourceUserId = routeValues[_parameterName]?.ToString();
        }
        else if (routeValues.ContainsKey("userId"))
        {
            resourceUserId = routeValues["userId"]?.ToString();
        }

        if (string.IsNullOrEmpty(resourceUserId) || resourceUserId != userIdClaim)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
