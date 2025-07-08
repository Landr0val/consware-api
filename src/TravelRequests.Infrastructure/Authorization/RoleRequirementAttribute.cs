using TravelRequests.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace TravelRequests.Infrastructure.Authorization;

public class RoleRequirementAttribute : Attribute, IAuthorizationFilter
{
    private readonly UserRole[] _allowedRoles;

    public RoleRequirementAttribute(params UserRole[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userRoleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userRoleClaim) || !Enum.TryParse<UserRole>(userRoleClaim, out var userRole))
        {
            context.Result = new ForbidResult();
            return;
        }

        if (!_allowedRoles.Contains(userRole))
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}

public class AprobadorOnlyAttribute : RoleRequirementAttribute
{
    public AprobadorOnlyAttribute() : base(UserRole.Aprobador) { }
}

public class SolicitanteOnlyAttribute : RoleRequirementAttribute
{
    public SolicitanteOnlyAttribute() : base(UserRole.Solicitante) { }
}

public class AnyRoleAttribute : RoleRequirementAttribute
{
    public AnyRoleAttribute() : base(UserRole.Solicitante, UserRole.Aprobador) { }
}
