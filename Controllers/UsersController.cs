using consware_api.Application.DTOs;
using consware_api.Application.Interfaces;
using consware_api.Domain.Enums;
using consware_api.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace consware_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [AprobadorOnly]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers([FromQuery] UserRole? role = null)
    {
        var users = await _userService.GetAllAsync(role);
        return Ok(users);
    }

    [HttpGet("{id}")]
    [ResourceOwner("id")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("email/{email}")]
    [AnyRole]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    [AprobadorOnly]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
    {
        try
        {
            var user = await _userService.CreateAsync(createUserDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [ResourceOwner("id")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _userService.UpdateAsync(id, updateUserDto);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [AprobadorOnly]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpPatch("{id}/change-password")]
    [ResourceOwner("id")]
    public async Task<ActionResult> ChangePassword(int id, ChangePasswordDto changePasswordDto)
    {
        try
        {
            var result = await _userService.ChangePasswordAsync(id, changePasswordDto);
            if (!result)
            {
                return BadRequest("No se pudo cambiar la contraseña");
            }
            return Ok(new { message = "Contraseña cambiada exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("request-password-reset")]
    [AnyRole]
    public async Task<ActionResult> RequestPasswordReset(PasswordResetRequestDto passwordResetRequestDto)
    {
        try
        {
            await _userService.RequestPasswordResetAsync(passwordResetRequestDto);
            return Ok(new { message = "Si el correo existe, recibirás un enlace para restablecer tu contraseña" });
        }
        catch (Exception)
        {
            return Ok(new { message = "Si el correo existe, recibirás un enlace para restablecer tu contraseña" });
        }
    }

    [HttpPost("reset-password")]
    [AnyRole]
    public async Task<ActionResult> ResetPassword(PasswordResetDto passwordResetDto)
    {
        try
        {
            var result = await _userService.ResetPasswordAsync(passwordResetDto);
            if (!result)
            {
                return BadRequest("Token inválido o expirado");
            }
            return Ok(new { message = "Contraseña restablecida exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
