using Microsoft.EntityFrameworkCore;
using WindTurbineMonitor.Api.Models;
using WindTurbineMonitor.Api.Services;

namespace WindTurbineMonitor.Api.Data;

public static class DatabaseInitializer
{
    private const string FarmId = "5e5789ff-d103-45f1-97bf-e8086254c02f";

    public static async Task SeedInitialDataAsync(AppDbContext db, ILogger logger, IAuthService authService)
    {
        try
        {
            // Seed test user if no users exist
            if (!await db.Users.AnyAsync())
            {
                var testUser = new User
                {
                    Username = "testuser",
                    PasswordHash = authService.HashPassword("password123"),
                    CreatedAt = DateTime.UtcNow
                };
                db.Users.Add(testUser);
                await db.SaveChangesAsync();
                logger.LogInformation("Seeded test user: testuser / password123");
            }

            // Only seed if no turbines exist
            if (await db.Turbines.AnyAsync())
            {
                logger.LogInformation("Database already contains turbines, skipping seed");
                return;
            }

            var turbines = new List<Turbine>
            {
                new()
                {
                    Id = "turbine-alpha",
                    Name = "Alpha",
                    Location = "North Platform",
                    MqttTopicPrefix = $"farm/{FarmId}/windmill/turbine-alpha",
                    InstalledAt = DateTime.UtcNow.AddYears(-2)
                },
                new()
                {
                    Id = "turbine-beta",
                    Name = "Beta",
                    Location = "North Platform",
                    MqttTopicPrefix = $"farm/{FarmId}/windmill/turbine-beta",
                    InstalledAt = DateTime.UtcNow.AddYears(-2)
                },
                new()
                {
                    Id = "turbine-gamma",
                    Name = "Gamma",
                    Location = "South Platform",
                    MqttTopicPrefix = $"farm/{FarmId}/windmill/turbine-gamma",
                    InstalledAt = DateTime.UtcNow.AddYears(-2)
                },
                new()
                {
                    Id = "turbine-delta",
                    Name = "Delta",
                    Location = "East Platform",
                    MqttTopicPrefix = $"farm/{FarmId}/windmill/turbine-delta",
                    InstalledAt = DateTime.UtcNow.AddYears(-2)
                }
            };

            db.Turbines.AddRange(turbines);
            await db.SaveChangesAsync();

            logger.LogInformation("Seeded 4 turbines into database");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to seed initial data");
            throw;
        }
    }
}
