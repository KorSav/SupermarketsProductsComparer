using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Testcontainers.PostgreSql;

namespace InfrastructureIntegrationTests;

public sealed class DbContainerFixture : IAsyncLifetime
{
    private PostgreSqlContainer _db = null!;

    public string AdminConnectionString = null!;
    public Logger Logger = null!;

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
            "logs",
            "output.log"
        );

        Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(logsPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();
        Logger.Information(new string('=', 50));
        Logger.Information("Starting new container");

        _db = new PostgreSqlBuilder("postgres:16-alpine")
            .WithLogger(
                LoggerFactory
                    .Create(builder =>
                    {
                        builder.AddSerilog(Logger).SetMinimumLevel(LogLevel.Trace);
                    })
                    .CreateLogger<PostgreSqlBuilder>()
            )
            .Build();
        await _db.StartAsync();
        AdminConnectionString = _db.GetConnectionString();
        Logger.Information(
            "Container is ready, admin connection string: {connString}",
            AdminConnectionString
        );
    }

    public async ValueTask DisposeAsync()
    {
        await _db.DisposeAsync();
        Logger.Information("Container disposed");
    }
}
