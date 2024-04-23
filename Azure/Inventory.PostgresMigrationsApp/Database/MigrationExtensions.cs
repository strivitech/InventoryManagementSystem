using Microsoft.EntityFrameworkCore;

namespace Inventory.PostgresMigrationsApp.Database;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
        var pendingMigrations = context.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            context.Database.Migrate();
        }
    }
}