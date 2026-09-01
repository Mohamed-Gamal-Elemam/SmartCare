using Microsoft.EntityFrameworkCore;
using SmartCare.Infrastructure.Persistence;
using SmartCare.Infrastructure.Persistence.Seeding;

namespace SmartCare.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<SmartCareDbContext>();
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();

        try
        {
            await context.Database.MigrateAsync();

            await SmartCareDataSeed.SeedAsync(
                context,
                services);
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogError(
                ex,
                "An error occurred while migrating or seeding the database.");
        }
    }
}