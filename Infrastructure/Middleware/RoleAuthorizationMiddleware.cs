using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace consware_api.Infrastructure.Middleware;

public class RoleAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public RoleAuthorizationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        var method = context.Request.Method;

        if (IsPublicEndpoint(path ?? "", method))
        {
            await _next(context);
            return;
        }

        var token = ExtractTokenFromHeader(context);
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token requerido");
            return;
        }

        var claims = ValidateToken(token);
        if (claims == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token inválido");
            return;
        }

        var userRole = claims.FindFirst(ClaimTypes.Role)?.Value ?? "";
        var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        if (!HasPermission(path ?? "", method, userRole, userId, context))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Acceso denegado");
            return;
        }

        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims.Claims, "jwt"));
        await _next(context);
    }

    private bool IsPublicEndpoint(string path, string method)
    {
        return path == "/api/auth/login" && method == "POST";
    }

    private string ExtractTokenFromHeader(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        return null!;
    }

    private ClaimsPrincipal ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "");
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null!;
        }
    }

    private bool HasPermission(string path, string method, string userRole, string userId, HttpContext context)
    {
        if (path.StartsWith("/api/users"))
        {
            if (method == "GET" && userRole == "Aprobador")
                return true;

            if (method == "POST" && userRole == "Aprobador")
                return true;

            if (method == "PUT" && userRole == "Aprobador")
                return true;

            if (method == "DELETE" && userRole == "Aprobador")
                return true;

            if (method == "PATCH" && path.Contains("/change-password"))
            {
                var pathSegments = path.Split('/');
                if (pathSegments.Length >= 4 && pathSegments[3] == userId)
                    return true;
            }

            if (method == "GET" && path.Contains("/email/"))
                return true;

            if (method == "POST" && (path.Contains("/request-password-reset") || path.Contains("/reset-password")))
                return true;
        }

        if (path.StartsWith("/api/travelrequests"))
        {
            if (method == "GET" && userRole == "Aprobador")
                return true;

            if (method == "GET" && path.Contains("/user/"))
            {
                var pathSegments = path.Split('/');
                if (pathSegments.Length >= 5 && pathSegments[4] == userId)
                    return true;
            }

            if (method == "POST" && path.Contains("/user/"))
            {
                var pathSegments = path.Split('/');
                if (pathSegments.Length >= 5 && pathSegments[4] == userId)
                    return true;
            }

            if (method == "PUT" && path.Contains("/status") && userRole == "Aprobador")
                return true;

            if (method == "DELETE" && userRole == "Aprobador")
                return true;
        }

        return false;
    }
}
