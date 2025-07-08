using consware_api.Application.DTOs;
using consware_api.Domain.Enums;

namespace consware_api.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllAsync(UserRole? role = null);
    Task<UserDto> CreateAsync(CreateUserDto createUserDto);
    Task<UserDto> UpdateAsync(int id, UpdateUserDto updateUserDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    Task<bool> RequestPasswordResetAsync(PasswordResetRequestDto passwordResetRequestDto);
    Task<bool> ResetPasswordAsync(PasswordResetDto passwordResetDto);
}
