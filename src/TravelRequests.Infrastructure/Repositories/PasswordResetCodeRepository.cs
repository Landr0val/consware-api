using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Repositories;
using TravelRequests.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TravelRequests.Infrastructure.Repositories;

public class PasswordResetCodeRepository : IPasswordResetCodeRepository
{
    private readonly AppDbContext _context;

    public PasswordResetCodeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PasswordResetCode> CreateAsync(PasswordResetCode resetCode)
    {
        _context.PasswordResetCodes.Add(resetCode);
        await _context.SaveChangesAsync();
        return resetCode;
    }

    public async Task<PasswordResetCode?> GetByCodeAndEmailAsync(string code, string email)
    {
        return await _context.PasswordResetCodes
            .Include(rc => rc.user)
            .FirstOrDefaultAsync(rc => rc.code == code && rc.email == email && !rc.used);
    }

    public async Task<PasswordResetCode?> GetByIdAsync(int id)
    {
        return await _context.PasswordResetCodes
            .Include(rc => rc.user)
            .FirstOrDefaultAsync(rc => rc.id == id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var resetCode = await _context.PasswordResetCodes.FindAsync(id);
        if (resetCode == null)
        {
            return false;
        }

        _context.PasswordResetCodes.Remove(resetCode);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> InvalidateByUserIdAsync(int userId)
    {
        var resetCodes = await _context.PasswordResetCodes
            .Where(rc => rc.user_id == userId && !rc.used)
            .ToListAsync();

        foreach (var resetCode in resetCodes)
        {
            resetCode.used = true;
            resetCode.used_at = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CleanExpiredCodesAsync()
    {
        try
        {
            var expiredCodes = await _context.PasswordResetCodes
                .Where(rc => rc.expires_at < DateTime.UtcNow)
                .ToListAsync();

            if (expiredCodes.Any())
            {
                _context.PasswordResetCodes.RemoveRange(expiredCodes);
                await _context.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<PasswordResetCode> UpdateAsync(PasswordResetCode resetCode)
    {
        _context.PasswordResetCodes.Update(resetCode);
        await _context.SaveChangesAsync();
        return resetCode;
    }
}
