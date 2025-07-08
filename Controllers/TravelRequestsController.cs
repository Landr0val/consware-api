using consware_api.Application.DTOs;
using consware_api.Application.Interfaces;
using consware_api.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace consware_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TravelRequestsController : ControllerBase
{
    private readonly ITravelRequestService _travelRequestService;
    private readonly IUserService _userService;

    public TravelRequestsController(ITravelRequestService travelRequestService, IUserService userService)
    {
        _travelRequestService = travelRequestService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TravelRequestDto>>> GetAllRequests()
    {
        var requests = await _travelRequestService.GetAllAsync();
        return Ok(requests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TravelRequestDto>> GetRequest(int id)
    {
        var request = await _travelRequestService.GetByIdAsync(id);
        if (request == null)
        {
            return NotFound();
        }
        return Ok(request);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<TravelRequestDto>>> GetRequestsByUser(int userId)
    {
        var requests = await _travelRequestService.GetByUserIdAsync(userId);
        return Ok(requests);
    }

    [HttpPost("user/{userId}")]
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

    [HttpPut("{id}/status")]
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

    [HttpDelete("{id}")]
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
