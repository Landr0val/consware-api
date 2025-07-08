using consware_api.Application.DTOs;
using consware_api.Application.Interfaces;
using consware_api.Domain.Entities;
using consware_api.Domain.Repositories;

namespace consware_api.Application.Services;

public class TravelRequestService : ITravelRequestService
{
    private readonly ITravelRequestRepository _travelRequestRepository;
    private readonly IUserRepository _userRepository;

    public TravelRequestService(ITravelRequestRepository travelRequestRepository, IUserRepository userRepository)
    {
        _travelRequestRepository = travelRequestRepository;
        _userRepository = userRepository;
    }

    public async Task<TravelRequestDto?> GetByIdAsync(int id)
    {
        var travelRequest = await _travelRequestRepository.GetByIdAsync(id);
        return travelRequest == null ? null : MapToDto(travelRequest);
    }

    public async Task<IEnumerable<TravelRequestDto>> GetAllAsync()
    {
        var travelRequests = await _travelRequestRepository.GetAllAsync();
        return travelRequests.Select(MapToDto);
    }

    public async Task<IEnumerable<TravelRequestDto>> GetByUserIdAsync(int userId)
    {
        var travelRequests = await _travelRequestRepository.GetByUserIdAsync(userId);
        return travelRequests.Select(MapToDto);
    }

    public async Task<TravelRequestDto> CreateAsync(int userId, CreateTravelRequestDto createTravelRequestDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("Usuario no encontrado");
        }

        if (createTravelRequestDto.return_date <= createTravelRequestDto.departure_date)
        {
            throw new InvalidOperationException("La fecha de regreso debe ser posterior a la fecha de ida");
        }

        if (createTravelRequestDto.origin_city.Equals(createTravelRequestDto.destination_city, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("La ciudad de origen debe ser diferente a la ciudad de destino");
        }

        var travelRequest = new TravelRequest
        {
            user_id = userId,
            origin_city = createTravelRequestDto.origin_city,
            destination_city = createTravelRequestDto.destination_city,
            departure_date = createTravelRequestDto.departure_date,
            return_date = createTravelRequestDto.return_date,
            justification = createTravelRequestDto.justification,
            created_at = DateTime.UtcNow
        };

        var createdTravelRequest = await _travelRequestRepository.CreateAsync(travelRequest);
        return MapToDto(createdTravelRequest);
    }

    public async Task<TravelRequestDto> UpdateStatusAsync(int id, UpdateRequestStatusDto updateRequestStatusDto)
    {
        var travelRequest = await _travelRequestRepository.GetByIdAsync(id);
        if (travelRequest == null)
        {
            throw new InvalidOperationException("Solicitud de viaje no encontrada");
        }

        travelRequest.status = updateRequestStatusDto.status;
        travelRequest.updated_at = DateTime.UtcNow;

        var updatedTravelRequest = await _travelRequestRepository.UpdateAsync(travelRequest);
        return MapToDto(updatedTravelRequest);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _travelRequestRepository.DeleteAsync(id);
    }

    private static TravelRequestDto MapToDto(TravelRequest travelRequest)
    {
        return new TravelRequestDto
        {
            id = travelRequest.id,
            user_id = travelRequest.user_id,
            user_name = travelRequest.user?.name ?? string.Empty,
            origin_city = travelRequest.origin_city,
            destination_city = travelRequest.destination_city,
            departure_date = travelRequest.departure_date,
            return_date = travelRequest.return_date,
            justification = travelRequest.justification,
            status = travelRequest.status,
            created_at = travelRequest.created_at,
            updated_at = travelRequest.updated_at
        };
    }
}
