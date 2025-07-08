using consware_api.Application.DTOs;
using consware_api.Domain.Entities;

namespace consware_api.Application.Interfaces;

public interface IJwtService
{
    Task<TokenResponseDto> GenerateTokenAsync(User user);
}
