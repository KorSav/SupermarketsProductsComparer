using ApplicationCore.Entities.Product;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApplicationCore.Services;

public class RepositoryFreshUpService(
    IServiceProvider serviceProvider,
    IOptionsMonitor<RepositoryFreshUpServiceOptions> optionsMonitor,
    ILogger<RepositoryFreshUpService> logger
) : BackgroundService
{
    private RepositoryFreshUpServiceOptions Options => optionsMonitor.CurrentValue;

    /// <summary>
    /// Logic of updating data in database regularly
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var parsingStartTime = DateTime.UtcNow; // TODO: store last parse time in db

            await using (var scope = serviceProvider.CreateAsyncScope())
            {
                var productRepo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
                var provider = scope.ServiceProvider.GetRequiredService<IShopProductProvider>();
                await RefreshRepositoryAsync(productRepo, provider, logger, cancellationToken);
            }

            var parsingDuration = DateTime.UtcNow - parsingStartTime;
            var remainingTime = Options.Interval - parsingDuration;
            if (remainingTime > TimeSpan.Zero)
            {
                logger.LogInformation(
                    "Start delay. Next cycle in {Time} (hh:mm:ss)",
                    remainingTime
                );
                await Task.Delay(remainingTime, cancellationToken);
            }
        }
    }

    private static async Task RefreshRepositoryAsync(
        IProductRepository productRepo,
        IShopProductProvider provider,
        ILogger<RepositoryFreshUpService> logger,
        CancellationToken ct
    )
    {
        var bulkScope = await productRepo.BeginBulkUpsertAsync(ct);
        try
        {
            logger.LogInformation("Starting new freshup cycle");
            const int bulkLoadSize = 100;
            int i = 1;
            List<Product> bulk = new(bulkLoadSize);
            await foreach (var product in provider.GetAllAsync(ct))
            {
                if (bulk.Count < bulkLoadSize)
                {
                    bulk.Add(product);
                    continue;
                }
                await bulkScope.UpsertAsync(bulk, ct);
                logger.LogInformation("{Iteration}). Saved {Count} new products", i++, bulk.Count);
                bulk.Clear();
            }
            if (bulk.Count > 0)
            {
                await bulkScope.UpsertAsync(bulk, ct);
                logger.LogInformation("{Iteration}). Saved {Count} new products", i++, bulk.Count);
            }
            logger.LogInformation("Refresh cycle finished");
            await bulkScope.CommitAsync(ct);
        }
        catch
        {
            await bulkScope.RollbackAsync();
            throw;
        }
    }
}
