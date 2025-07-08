using consware_api.Application.DTOs;
using consware_api.Application.Interfaces;
using consware_api.Domain.Enums;
using consware_api.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace consware_api.Controllers;

/// <summary>
/// Controlador para gestión de usuarios
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]

public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Obtiene todos los usuarios, opcionalmente filtrados por rol
    /// </summary>
    /// <param name="role">Rol para filtrar usuarios (opcional)</param>
    /// <returns>Lista de usuarios</returns>
    /// <response code="200">Lista de usuarios obtenida exitosamente</response>
    /// <response code="401">No autorizado</response>
    /// <response code="403">Acceso denegado</response>
    [HttpGet]
    [AprobadorOnly]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]

    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers([FromQuery] UserRole? role = null)
    {
        var users = await _userService.GetAllAsync(role);
        return Ok(users);
    }

    /// <summary>
    /// Obtiene un usuario específico por su ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Usuario encontrado</returns>
    /// <response code="200">Usuario encontrado</response>
    /// <response code="401">No autorizado</response>
    /// <response code="403">Acceso denegado</response>
    /// <response code="404">Usuario no encontrado</response>
    [HttpGet("{id}")]
    [ResourceOwner("id")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]

    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    /// <summary>
    /// Obtiene un usuario específico por su dirección de correo electrónico
    /// </summary>
    /// <param name="email">Dirección de correo electrónico</param>
    /// <returns>Usuario encontrado</returns>
    /// <response code="200">Usuario encontrado</response>
    /// <response code="401">No autorizado</response>
    /// <response code="404">Usuario no encontrado</response>
    [HttpGet("email/{email}")]
    [AnyRole]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]

    public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    /// <summary>
    /// Crea un nuevo usuario en el sistema
    /// </summary>
    /// <param name="createUserDto">Datos del usuario a crear</param>
    /// <returns>Usuario creado</returns>
    /// <response code="201">Usuario creado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="401">No autorizado</response>
    /// <response code="403">Acceso denegado</response>
    [HttpPost]
    [AprobadorOnly]
    [ProducesResponseType(typeof(UserDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]

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

    /// <summary>
    /// Actualiza la información de un usuario existente
    /// </summary>
    /// <param name="id">ID del usuario a actualizar</param>
    /// <param name="updateUserDto">Datos actualizados del usuario</param>
    /// <returns>Usuario actualizado</returns>
    /// <response code="200">Usuario actualizado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="401">No autorizado</response>
    /// <response code="403">Acceso denegado</response>
    /// <response code="404">Usuario no encontrado</response>
    [HttpPut("{id}")]
    [ResourceOwner("id")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]

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

    /// <summary>
    /// Elimina un usuario del sistema
    /// </summary>
    /// <param name="id">ID del usuario a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="204">Usuario eliminado exitosamente</response>
    /// <response code="401">No autorizado</response>
    /// <response code="403">Acceso denegado</response>
    /// <response code="404">Usuario no encontrado</response>
    [HttpDelete("{id}")]
    [AprobadorOnly]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]

    public async Task<ActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    /// <summary>
    /// Cambia la contraseña de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="changePasswordDto">Datos para cambio de contraseña</param>
    /// <returns>Confirmación de cambio de contraseña</returns>
    /// <response code="200">Contraseña cambiada exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="401">No autorizado</response>
    /// <response code="403">Acceso denegado</response>
    /// <response code="404">Usuario no encontrado</response>
    [HttpPatch("{id}/change-password")]
    [ResourceOwner("id")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]

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

    /// <summary>
    /// Solicita el restablecimiento de contraseña
    /// </summary>
    /// <param name="passwordResetRequestDto">Datos para solicitud de restablecimiento</param>
    /// <returns>Confirmación de solicitud procesada</returns>
    /// <response code="200">Solicitud procesada exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    [HttpPost("request-password-reset")]
    [AnyRole]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]

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

    /// <summary>
    /// Restablece la contraseña de un usuario usando un token de restablecimiento
    /// </summary>
    /// <param name="passwordResetDto">Datos para restablecimiento de contraseña</param>
    /// <returns>Confirmación de restablecimiento</returns>
    /// <response code="200">Contraseña restablecida exitosamente</response>
    /// <response code="400">Token inválido o expirado</response>
    [HttpPost("reset-password")]
    [AnyRole]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]

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
