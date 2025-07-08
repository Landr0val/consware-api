using TravelRequests.Application.DTOs;
using TravelRequests.Domain.Entities;

namespace TravelRequests.Application.Interfaces;

public interface IJwtService
{
    Task<TokenResponseDto> GenerateTokenAsync(User user);
}
