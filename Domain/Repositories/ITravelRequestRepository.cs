using consware_api.Domain.Entities;

namespace consware_api.Domain.Repositories;

public interface ITravelRequestRepository
{
    Task<TravelRequest?> GetByIdAsync(int id);
    Task<IEnumerable<TravelRequest>> GetAllAsync();
    Task<IEnumerable<TravelRequest>> GetByUserIdAsync(int userId);
    Task<TravelRequest> CreateAsync(TravelRequest travelRequest);
    Task<TravelRequest> UpdateAsync(TravelRequest travelRequest);
    Task<bool> DeleteAsync(int id);
}
