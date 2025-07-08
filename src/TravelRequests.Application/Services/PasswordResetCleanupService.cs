using TravelRequests.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TravelRequests.Application.Services;

public class PasswordResetCleanupService : BackgroundService
{
    private readonly ILogger<PasswordResetCleanupService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(10);

    public PasswordResetCleanupService(
        ILogger<PasswordResetCleanupService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Password reset cleanup service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredCodes();
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Password reset cleanup service is stopping.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password reset codes cleanup");
                try
                {
                    await Task.Delay(_cleanupInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Password reset cleanup service is stopping.");
                    break;
                }
            }
        }
    }

    private async Task CleanupExpiredCodes()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var resetCodeRepository = scope.ServiceProvider.GetRequiredService<IPasswordResetCodeRepository>();

            var result = await resetCodeRepository.CleanExpiredCodesAsync();

            if (result)
            {
                _logger.LogInformation("Expired password reset codes cleaned up successfully at {Timestamp}", DateTime.UtcNow);
            }
            else
            {
                _logger.LogWarning("Password reset codes cleanup returned false, possibly due to database connectivity issues");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clean up expired password reset codes");
            throw;
        }
    }
}
