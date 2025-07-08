using consware_api.Application.DTOs;
using consware_api.Application.Interfaces;
using consware_api.Domain.Repositories;

namespace consware_api.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
        _jwtService = jwtService;
    }

    public async Task<TokenResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.email);

        if (user == null || !user.active)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        if (!_passwordHashingService.VerifyPassword(loginDto.password, user.password))
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        return await _jwtService.GenerateTokenAsync(user);
    }
}
