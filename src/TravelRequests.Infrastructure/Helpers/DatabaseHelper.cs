using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelRequests.Infrastructure.Data;

namespace TravelRequests.Infrastructure.Helpers;

public static class DatabaseHelper
{
    public static async Task<bool> EnsureDatabaseExistsAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                logger.LogWarning("Cannot connect to database, attempting to create it");
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("Database created successfully");
            }
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to ensure database exists");
            return false;
        }
    }

    public static async Task<bool> ApplyMigrationsAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation("Found {Count} pending migrations: {Migrations}",
                    pendingMigrations.Count(),
                    string.Join(", ", pendingMigrations));

                await context.Database.MigrateAsync();
                logger.LogInformation("All pending migrations applied successfully");
            }
            else
            {
                logger.LogInformation("No pending migrations found");
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply migrations");
            return false;
        }
    }

    public static async Task<bool> InitializeDatabaseAsync(AppDbContext context, ILogger logger, int maxRetries = 10)
    {
        var retryDelay = TimeSpan.FromSeconds(5);

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                logger.LogInformation("Attempting to initialize database (attempt {Attempt}/{MaxRetries})", i + 1, maxRetries);

                if (await EnsureDatabaseExistsAsync(context, logger))
                {
                    if (await ApplyMigrationsAsync(context, logger))
                    {
                        logger.LogInformation("Database initialized successfully");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Database initialization failed on attempt {Attempt}/{MaxRetries}", i + 1, maxRetries);
            }

            if (i < maxRetries - 1)
            {
                logger.LogInformation("Retrying in {Delay} seconds...", retryDelay.TotalSeconds);
                await Task.Delay(retryDelay);
            }
        }

        logger.LogError("Failed to initialize database after {MaxRetries} attempts", maxRetries);
        return false;
    }
}
