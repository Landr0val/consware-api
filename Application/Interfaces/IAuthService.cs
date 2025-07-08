using consware_api.Application.DTOs;

namespace consware_api.Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginDto loginDto);
}
