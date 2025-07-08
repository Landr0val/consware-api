using consware_api.Application.DTOs;
using consware_api.Application.Interfaces;
using consware_api.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace consware_api.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<TokenResponseDto> GenerateTokenAsync(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.id.ToString()),
            new(ClaimTypes.Email, user.email),
            new(ClaimTypes.Name, user.name),
            new(ClaimTypes.Role, user.role.ToString())
        };

        var expiresAt = DateTime.UtcNow.AddHours(24);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult(new TokenResponseDto
        {
            token = tokenString,
            role = user.role.ToString(),
            expires_at = expiresAt,
            email = user.email
        });
    }
}
