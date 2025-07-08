using TravelRequests.Application.Interfaces;
using TravelRequests.Domain.Enums;
using TravelRequests.Infrastructure.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace TravelRequests.Infrastructure.Middleware;

public class ResourceAccessMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public ResourceAccessMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        var method = context.Request.Method;

        if (RequiresResourceCheck(path ?? "", method))
        {
            var hasAccess = await CheckResourceAccess(context, path ?? "", method);
            if (!hasAccess)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Acceso denegado al recurso");
                return;
            }
        }

        await _next(context);
    }

    private bool RequiresResourceCheck(string path, string method)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        if (path.StartsWith("/api/travelrequests/") && method == "GET" && !path.Contains("/user/"))
            return true;

        if (path.StartsWith("/api/travelrequests/") && method == "PUT" && path.Contains("/status"))
            return true;

        if (path.StartsWith("/api/travelrequests/") && method == "DELETE")
            return true;

        return false;
    }

    private async Task<bool> CheckResourceAccess(HttpContext context, string? path, string method)
    {
        if (!UserContextHelper.IsAuthenticated(context))
            return false;

        if (UserContextHelper.IsCurrentUserApprover(context))
            return true;

        var userId = UserContextHelper.GetCurrentUserId(context);
        if (!userId.HasValue)
            return false;

        if (string.IsNullOrEmpty(path))
            return false;

        using var scope = _serviceProvider.CreateScope();
        var travelRequestService = scope.ServiceProvider.GetRequiredService<ITravelRequestService>();

        var pathSegments = path.Split('/');
        if (pathSegments.Length < 4)
            return false;

        if (!int.TryParse(pathSegments[3], out int requestId))
            return false;

        var request = await travelRequestService.GetByIdAsync(requestId);
        if (request == null)
            return false;

        return UserContextHelper.IsResourceOwner(context, request.user_id);
    }
}
