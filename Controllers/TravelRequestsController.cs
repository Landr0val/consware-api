using consware_api.Application.DTOs;
using consware_api.Application.Interfaces;
using consware_api.Domain.Enums;
using consware_api.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace consware_api.Controllers;

/// <summary>
/// Controlador para gestión de solicitudes de viaje
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TravelRequestsController : ControllerBase
{
    private readonly ITravelRequestService _travelRequestService;
    private readonly IUserService _userService;

    public TravelRequestsController(ITravelRequestService travelRequestService, IUserService userService)
    {
        _travelRequestService = travelRequestService;
        _userService = userService;
    }

    /// <summary>
    /// Obtiene todas las solicitudes de viaje
    /// </summary>
    /// <returns>Lista de todas las solicitudes de viaje</returns>
    /// <response code="200">Lista de solicitudes obtenida exitosamente</response>
    /// <response code="401">No autorizado</response>
    /// <response code="403">Acceso denegado</response>
    [HttpGet]
    [AprobadorOnly]
    [ProducesResponseType(typeof(IEnumerable<TravelRequestDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<TravelRequestDto>>> GetAllRequests()
    {
        var requests = await _travelRequestService.GetAllAsync();
        return Ok(requests);
    }

    /// <summary>
    /// Obtiene una solicitud de viaje específica por su ID
    /// </summary>
    /// <param name="id">ID de la solicitud</param>
    /// <returns>Solicitud de viaje encontrada</returns>
    /// <response code="200">Solicitud encontrada</response>
    /// <response code="401">No autorizado</response>
    /// <response code="404">Solicitud no encontrada</response>
    [HttpGet("{id}")]
    [AnyRole]
    [ProducesResponseType(typeof(TravelRequestDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<TravelRequestDto>> GetRequest(int id)
    {
        var request = await _travelRequestService.GetByIdAsync(id);
        if (request == null)
        {
            return NotFound();
        }
        return Ok(request);
    }

    /// <summary>
    /// Obtiene todas las solicitudes de viaje de un usuario específico
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Lista de solicitudes del usuario</returns>
    /// <response code="200">Lista de solicitudes del usuario obtenida exitosamente</response>
    /// <response code="401">No autorizado</response>
    /// <response code="403">Acceso denegado</response>
    [HttpGet("user/{userId}")]
    [ResourceOwner("userId")]
    [ProducesResponseType(typeof(IEnumerable<TravelRequestDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<TravelRequestDto>>> GetRequestsByUser(int userId)
    {
        var requests = await _travelRequestService.GetByUserIdAsync(userId);
        return Ok(requests);
    }

    /// <summary>
    /// Crea una nueva solicitud de viaje para un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="createTravelRequestDto">Datos de la solicitud a crear</param>
    /// <returns>Solicitud creada</returns>
    /// <response code="201">Solicitud creada exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="401">No autorizado</response>
    /// <response code="403">Acceso denegado</response>
    [HttpPost("user/{userId}")]
    [ResourceOwner("userId")]
    [ProducesResponseType(typeof(TravelRequestDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<TravelRequestDto>> CreateRequest(int userId, CreateTravelRequestDto createTravelRequestDto)
    {
        try
        {
            var request = await _travelRequestService.CreateAsync(userId, createTravelRequestDto);
            return CreatedAtAction(nameof(GetRequest), new { id = request.id }, request);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza el estado de una solicitud de viaje (aprobar/rechazar)
    /// </summary>
    /// <param name="id">ID de la solicitud</param>
    /// <param name="updateRequestStatusDto">Datos de actualización de estado</param>
    /// <param name="approverId">ID del usuario aprobador</param>
    /// <returns>Solicitud actualizada</returns>
    /// <response code="200">Estado de solicitud actualizado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="401">No autorizado</response>
    /// <response code="403">Acceso denegado</response>
    /// <response code="404">Solicitud no encontrada</response>
    [HttpPut("{id}/status")]
    [AprobadorOnly]
    [ProducesResponseType(typeof(TravelRequestDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<TravelRequestDto>> UpdateRequestStatus(int id, UpdateRequestStatusDto updateRequestStatusDto, [FromQuery] int approverId)
    {
        try
        {
            var approver = await _userService.GetByIdAsync(approverId);
            if (approver == null || approver.role != UserRole.Aprobador)
            {
                return Forbid("Solo los usuarios con rol Aprobador pueden cambiar el estado de las solicitudes");
            }

            var request = await _travelRequestService.UpdateStatusAsync(id, updateRequestStatusDto);
            return Ok(request);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina una solicitud de viaje del sistema
    /// </summary>
    /// <param name="id">ID de la solicitud a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="204">Solicitud eliminada exitosamente</response>
    /// <response code="401">No autorizado</response>
    /// <response code="403">Acceso denegado</response>
    /// <response code="404">Solicitud no encontrada</response>
    [HttpDelete("{id}")]
    [AprobadorOnly]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteRequest(int id)
    {
        var result = await _travelRequestService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}
