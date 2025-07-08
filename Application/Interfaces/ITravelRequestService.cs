using consware_api.Application.DTOs;

namespace consware_api.Application.Interfaces;

public interface ITravelRequestService
{
    Task<TravelRequestDto?> GetByIdAsync(int id);
    Task<IEnumerable<TravelRequestDto>> GetAllAsync();
    Task<IEnumerable<TravelRequestDto>> GetByUserIdAsync(int userId);
    Task<TravelRequestDto> CreateAsync(int userId, CreateTravelRequestDto createTravelRequestDto);
    Task<TravelRequestDto> UpdateStatusAsync(int id, UpdateRequestStatusDto updateRequestStatusDto);
    Task<bool> DeleteAsync(int id);
}
