using TravelRequests.Domain.Entities;

namespace TravelRequests.Domain.Repositories;

public interface IPasswordResetCodeRepository
{
    Task<PasswordResetCode> CreateAsync(PasswordResetCode resetCode);
    Task<PasswordResetCode?> GetByCodeAndEmailAsync(string code, string email);
    Task<PasswordResetCode?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<bool> InvalidateByUserIdAsync(int userId);
    Task<bool> CleanExpiredCodesAsync();
    Task<PasswordResetCode> UpdateAsync(PasswordResetCode resetCode);
}
