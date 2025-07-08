using TravelRequests.Application.DTOs;

namespace TravelRequests.Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginDto loginDto);
}
