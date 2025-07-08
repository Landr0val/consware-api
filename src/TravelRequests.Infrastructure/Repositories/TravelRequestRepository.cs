using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Repositories;
using TravelRequests.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TravelRequests.Infrastructure.Repositories;

public class TravelRequestRepository : ITravelRequestRepository
{
    private readonly AppDbContext _context;

    public TravelRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TravelRequest?> GetByIdAsync(int id)
    {
        return await _context.TravelRequests
            .Include(tr => tr.user)
            .FirstOrDefaultAsync(tr => tr.id == id);
    }

    public async Task<IEnumerable<TravelRequest>> GetAllAsync()
    {
        return await _context.TravelRequests
            .Include(tr => tr.user)
            .ToListAsync();
    }

    public async Task<IEnumerable<TravelRequest>> GetByUserIdAsync(int userId)
    {
        return await _context.TravelRequests
            .Include(tr => tr.user)
            .Where(tr => tr.user_id == userId)
            .ToListAsync();
    }

    public async Task<TravelRequest> CreateAsync(TravelRequest travelRequest)
    {
        _context.TravelRequests.Add(travelRequest);
        await _context.SaveChangesAsync();
        return travelRequest;
    }

    public async Task<TravelRequest> UpdateAsync(TravelRequest travelRequest)
    {
        _context.TravelRequests.Update(travelRequest);
        await _context.SaveChangesAsync();
        return travelRequest;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var travelRequest = await _context.TravelRequests.FindAsync(id);
        if (travelRequest == null)
        {
            return false;
        }

        _context.TravelRequests.Remove(travelRequest);
        await _context.SaveChangesAsync();
        return true;
    }
}
