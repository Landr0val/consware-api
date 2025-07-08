using consware_api.Application.DTOs;
using consware_api.Application.Interfaces;
using consware_api.Domain.Entities;
using consware_api.Domain.Repositories;

namespace consware_api.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;

    public UserService(IUserRepository userRepository, IPasswordHashingService passwordHashingService)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
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

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
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
