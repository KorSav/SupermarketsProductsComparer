using ApplicationCore;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Serilog;

namespace InfrastructureIntegrationTests;

public abstract class DbPerTestCaseBase(DbContainerFixture fixture) : IAsyncLifetime
{
    private IHost _host = null!;
    private string _dbName = null!;

    public static string TCName => TestContext.Current.TestCase!.TestCaseDisplayName;
    public static CancellationToken CancellationToken => TestContext.Current.CancellationToken;
    public IServiceProvider Services { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        _dbName = $"db_{GetType().Name}_{Guid.NewGuid():N}";
        fixture.Logger.Information("Creating database {DB} for TC {TC}", _dbName, TCName);
        await using (var adminConnection = new NpgsqlConnection(fixture.AdminConnectionString))
        {
            await adminConnection.OpenAsync();
            using var cmd = new NpgsqlCommand($"CREATE DATABASE {_dbName}", adminConnection);
            await cmd.ExecuteNonQueryAsync();
        }
        var connString = new NpgsqlConnectionStringBuilder(fixture.AdminConnectionString)
        {
            Database = _dbName,
        }.ConnectionString;
        fixture.Logger.Information(
            "{DB} created, connection string: {connString}",
            _dbName,
            connString
        );

        var builder = Host.CreateEmptyApplicationBuilder(null);
        builder
            .Configuration.AddJsonFile("./RepositoryTests/appsettings.json")
            .AddInMemoryCollection([new("ConnectionStrings:DefaultConnection", connString)]);
        builder.Services.AddLogging(builder => builder.AddSerilog(fixture.Logger));
        builder.AddApplicationCore();
        builder.AddInfrastructure();

        _host = builder.Build();
        fixture.Logger.Information("{DB} built IHost, creating tables", _dbName);
        await using (var scope = _host.Services.CreateAsyncScope())
        {
            var dbCtx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbCtx.Database.EnsureCreatedAsync();
        }
        fixture.Logger.Information("{DB} start executing TC", _dbName);
        Services = _host.Services;
    }

    public async ValueTask DisposeAsync()
    {
        _host.Dispose();
        await using var adminConnection = new NpgsqlConnection(fixture.AdminConnectionString);
        await adminConnection.OpenAsync();
        using var cmd = new NpgsqlCommand($"DROP DATABASE {_dbName} WITH (Force)", adminConnection);
        await cmd.ExecuteNonQueryAsync();
        fixture.Logger.Information("{DB} and host disposed", _dbName);
        GC.SuppressFinalize(this);
    }
}
