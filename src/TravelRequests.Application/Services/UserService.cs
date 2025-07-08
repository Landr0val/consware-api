using TravelRequests.Application.DTOs;
using TravelRequests.Application.Interfaces;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using TravelRequests.Domain.Repositories;

namespace TravelRequests.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IPasswordResetCodeRepository _passwordResetCodeRepository;

    public UserService(IUserRepository userRepository, IPasswordHashingService passwordHashingService, IPasswordResetCodeRepository passwordResetCodeRepository)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
        _passwordResetCodeRepository = passwordResetCodeRepository;
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(UserRole? role = null)
    {
        var users = await _userRepository.GetAllAsync();
        if (role.HasValue)
        {
            users = users.Where(u => u.role == role.Value);
        }
        return users.Select(MapToDto);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
    {
        if (await _userRepository.ExistsAsync(createUserDto.email))
        {
            throw new InvalidOperationException("Ya existe un usuario con este correo electrónico");
        }

        var user = new User
        {
            name = createUserDto.name,
            email = createUserDto.email,
            password = _passwordHashingService.HashPassword(createUserDto.password),
            role = createUserDto.role,
            created_at = DateTime.UtcNow,
            active = true
        };

        var createdUser = await _userRepository.CreateAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<UserDto> UpdateAsync(int id, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException("Usuario no encontrado");
        }

        if (user.email != updateUserDto.email && await _userRepository.ExistsAsync(updateUserDto.email))
        {
            throw new InvalidOperationException("Ya existe un usuario con este correo electrónico");
        }

        user.name = updateUserDto.name;
        user.email = updateUserDto.email;
        user.role = updateUserDto.role;
        user.active = updateUserDto.active;
        user.updated_at = DateTime.UtcNow;

        var updatedUser = await _userRepository.UpdateAsync(user);
        return MapToDto(updatedUser);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("Usuario no encontrado");
        }

        if (!_passwordHashingService.VerifyPassword(changePasswordDto.currentPassword, user.password))
        {
            throw new InvalidOperationException("La contraseña actual es incorrecta");
        }

        user.password = _passwordHashingService.HashPassword(changePasswordDto.newPassword);
        user.updated_at = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<PasswordResetRequestResponseDto> RequestPasswordResetAsync(PasswordResetRequestDto passwordResetRequestDto)
    {
        var user = await _userRepository.GetByEmailAsync(passwordResetRequestDto.email);
        if (user == null)
        {
            throw new InvalidOperationException("No se encontró un usuario con este correo electrónico");
        }

        if (!user.active)
        {
            throw new InvalidOperationException("La cuenta está desactivada");
        }

        await _passwordResetCodeRepository.InvalidateByUserIdAsync(user.id);
        await _passwordResetCodeRepository.CleanExpiredCodesAsync();

        var code = GenerateVerificationCode();
        var expiresAt = DateTime.UtcNow.AddMinutes(5);

        var resetCode = new PasswordResetCode
        {
            user_id = user.id,
            code = code,
            email = user.email,
            created_at = DateTime.UtcNow,
            expires_at = expiresAt,
            used = false
        };

        await _passwordResetCodeRepository.CreateAsync(resetCode);

        return new PasswordResetRequestResponseDto
        {
            message = "Código de verificación generado exitosamente",
            code = code,
            email = user.email,
            expires_at = expiresAt
        };
    }

    public async Task<bool> ResetPasswordAsync(PasswordResetDto passwordResetDto)
    {
        var resetCode = await _passwordResetCodeRepository.GetByCodeAndEmailAsync(passwordResetDto.code, passwordResetDto.email);

        if (resetCode == null)
        {
            throw new InvalidOperationException("Código de verificación inválido o correo no coincide");
        }

        if (resetCode.used)
        {
            throw new InvalidOperationException("El código ya ha sido utilizado");
        }

        if (resetCode.expires_at < DateTime.UtcNow)
        {
            throw new InvalidOperationException("El código ha expirado");
        }

        var user = resetCode.user;
        if (user == null || !user.active)
        {
            throw new InvalidOperationException("Usuario no encontrado o cuenta desactivada");
        }

        user.password = _passwordHashingService.HashPassword(passwordResetDto.newPassword);
        user.updated_at = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        resetCode.used = true;
        resetCode.used_at = DateTime.UtcNow;
        await _passwordResetCodeRepository.UpdateAsync(resetCode);

        await _passwordResetCodeRepository.InvalidateByUserIdAsync(user.id);

        return true;
    }

    private static string GenerateVerificationCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            id = user.id,
            name = user.name,
            email = user.email,
            role = user.role,
            created_at = user.created_at,
            updated_at = user.updated_at,
            active = user.active
        };
    }
}
