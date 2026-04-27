using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceProviderExtensions
{
    public static async Task ThrowIfDbIsNotAccessibleAsync(this IServiceProvider sp)
    {
        // Simple check to catch invalid startup, enforces db to be accessible in a valid state.
        // There are no any validation for db failure after app started, might be implemented in future
        await using var scope = sp.CreateAsyncScope();
        var ctx = sp.GetRequiredService<AppDbContext>();
        if (!await ctx.Database.CanConnectAsync())
        {
            throw new InvalidOperationException(
                "Make sure that DB is created and running. Can't start application since DB is not reachable"
            );
        }
        var notAppliedMigrations = await ctx.Database.GetPendingMigrationsAsync();
        if (notAppliedMigrations.Any())
        {
            throw new InvalidOperationException(
                "There are pending migrations, DB has old state. Please apply migrations using 'dotnet ef database update'"
            );
        }
    }
}
