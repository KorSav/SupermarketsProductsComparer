using ApplicationCore;
using ApplicationCore.Entities.Product;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;
using Testcontainers.PostgreSql;

namespace InfrastructureIntegrationTests.RepositoryTests;

public sealed class ProductRepositoryTests(DbContainerFixture _)
    : DbTestBase(_),
        IClassFixture<DbContainerFixture>
{
    [Fact]
    public async Task UpsertAsync_InsertsDataAndHasNoLeftoverTable()
    {
        // Arrange
        await using (var scope = Services.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var bulkScope = await repo.BeginBulkUpsertAsync(CancellationToken);

            // Act
            await bulkScope.UpsertAsync(New3Products(), CancellationToken);
            await bulkScope.CommitAsync(CancellationToken);
        }

        // Assert
        await using (var scope = Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var expected = New3Products();
            var actual = await dbContext
                .Products.Select(p => p.ToCoreProduct())
                .ToListAsync(CancellationToken);
            Assert.Equivalent(expected, actual);
        }
    }

    private List<Product> New3Products() =>
        [
            new Product(
                Name: "Молоко",
                Measure: new(1, MeasureUnit.Litre),
                Price: 30,
                LinkImage: new Uri("https://non-existing-images-silpo.com"),
                LinkProduct: new Uri("https://non-existing-products-silpo.com"),
                Shop: Shop.Silpo
            ),
            new Product(
                Name: "Apple",
                Measure: new(500, MeasureUnit.Gram),
                Price: 15,
                LinkImage: new Uri("https://image-stock.com"),
                LinkProduct: new Uri("https://non-existing-products-fozzy.com"),
                Shop: Shop.Fozzy
            ),
            new Product(
                Name: "Напій juice",
                Measure: new(200, MeasureUnit.MiliLitre),
                Price: 20,
                LinkImage: new Uri("https://image-stock.com"),
                LinkProduct: new Uri("https://non-existing-products-fora.com"),
                Shop: Shop.Fora
            ),
        ];
}

public sealed class DbContainerFixture : IAsyncLifetime
{
    private PostgreSqlContainer _db = null!;

    public string AdminConnectionString = null!;

    public async ValueTask InitializeAsync()
    {
        var logsPath = Path.Combine(
            "E:",
            "sava",
            "education",
            "diploma",
            "project",
            "test",
            "InfrastructureIntegrationTests",
            "output.log"
        );

        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.File(logsPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();
        serilogLogger.Information("Starting db");

        _db = new PostgreSqlBuilder("postgres:16-alpine")
            .WithLogger(
                LoggerFactory
                    .Create(builder =>
                    {
                        builder.AddSerilog(serilogLogger).SetMinimumLevel(LogLevel.Trace);
                    })
                    .CreateLogger<PostgreSqlBuilder>()
            )
            .Build();
        await _db.StartAsync();
        AdminConnectionString = _db.GetConnectionString();
        serilogLogger.Information("Db is running, configuring app");
    }

    public async ValueTask DisposeAsync()
    {
        await _db.DisposeAsync();
    }
}

public abstract class DbTestBase(DbContainerFixture fixture)
    : IAsyncLifetime,
        IClassFixture<DbContainerFixture>
{
    private IHost _host = null!;
    private string _dbName = null!;

    public static CancellationToken CancellationToken => TestContext.Current.CancellationToken;
    public IServiceProvider Services { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        _dbName = $"db_{GetType().Name}_{Guid.NewGuid():N}";
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

        var builder = Host.CreateEmptyApplicationBuilder(null);
        builder
            .Configuration.AddJsonFile("./RepositoryTests/appsettings.json")
            .AddInMemoryCollection([new("ConnectionStrings:DefaultConnection", connString)]);
        builder.AddInfrastructure();

        _host = builder.Build();
        await using (var scope = _host.Services.CreateAsyncScope())
        {
            var dbCtx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbCtx.Database.EnsureCreatedAsync();
        }
        Services = _host.Services;
    }

    public async ValueTask DisposeAsync()
    {
        _host.Dispose();
        await using var adminConnection = new NpgsqlConnection(fixture.AdminConnectionString);
        await adminConnection.OpenAsync();
        using var cmd = new NpgsqlCommand($"DROP DATABASE {_dbName} WITH (Force)", adminConnection);
        await cmd.ExecuteNonQueryAsync();
        GC.SuppressFinalize(this);
    }
}
