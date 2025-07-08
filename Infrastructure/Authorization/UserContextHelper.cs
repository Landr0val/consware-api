using System.Security.Claims;
using consware_api.Domain.Enums;

namespace consware_api.Infrastructure.Authorization;

public static class UserContextHelper
{
    public static int? GetCurrentUserId(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : null;
    }

    public static string GetCurrentUserEmail(HttpContext context)
    {
        return context.User.FindFirst(ClaimTypes.Email)?.Value;
    }

    public static string GetCurrentUserName(HttpContext context)
    {
        return context.User.FindFirst(ClaimTypes.Name)?.Value;
    }

    public static UserRole? GetCurrentUserRole(HttpContext context)
    {
        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;
        return Enum.TryParse<UserRole>(roleClaim, out var role) ? role : null;
    }

    public static bool IsCurrentUserApprover(HttpContext context)
    {
        return GetCurrentUserRole(context) == UserRole.Aprobador;
    }

    public static bool IsCurrentUserRequester(HttpContext context)
    {
        return GetCurrentUserRole(context) == UserRole.Solicitante;
    }

    public static bool IsAuthenticated(HttpContext context)
    {
        return context.User.Identity?.IsAuthenticated ?? false;
    }

    public static bool IsResourceOwner(HttpContext context, int resourceUserId)
    {
        var currentUserId = GetCurrentUserId(context);
        return currentUserId.HasValue && currentUserId.Value == resourceUserId;
    }

    public static bool CanAccessResource(HttpContext context, int resourceUserId)
    {
        return IsCurrentUserApprover(context) || IsResourceOwner(context, resourceUserId);
    }
}
