using System.Runtime.CompilerServices;
using ApplicationCore.Entities.Product;
using ApplicationCore.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ApplicationCore.UnitTests.Services;

public sealed class RepositoryFreshUpServiceTests
{
    [Fact]
    public async Task ExecuteAsync_UpsertsProductsInBatchesAndCommitsWhenProviderCompletesSuccessfully()
    {
        using var cts = new CancellationTokenSource();
        var products = Enumerable.Range(0, 205).Select(i => Product($"Product {i}")).ToList();
        var bulkScope = Substitute.For<IBulkUpsertScope>();
        var productRepo = Substitute.For<IProductRepository>();
        var provider = Substitute.For<IShopProductProvider>();
        var service = CreateService(productRepo, provider, interval: TimeSpan.Zero);
        productRepo
            .BeginBulkUpsertAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(bulkScope));
        provider
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(callInfo =>
                ToAsyncEnumerable(products, cts.Cancel, callInfo.Arg<CancellationToken>())
            );
        var upsertedBatches = new List<IReadOnlyCollection<Product>>();
        bulkScope
            .When(x =>
                x.UpsertAsync(Arg.Any<IReadOnlyCollection<Product>>(), Arg.Any<CancellationToken>())
            )
            .Do(callInfo =>
                upsertedBatches.Add(callInfo.Arg<IReadOnlyCollection<Product>>().ToList())
            );

        await service.ExecuteForTestAsync(cts.Token);

        await productRepo.Received(1).BeginBulkUpsertAsync(Arg.Any<CancellationToken>());
        Assert.Equal(3, upsertedBatches.Count);
        Assert.Equal(100, upsertedBatches[0].Count);
        Assert.Equal(100, upsertedBatches[1].Count);
        Assert.Equal(5, upsertedBatches[2].Count);
        await bulkScope.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        await bulkScope.DidNotReceive().RollbackAsync();
    }

    [Fact]
    public async Task ExecuteAsync_RollsBackAndRethrowsWhenUpsertFails()
    {
        using var cts = new CancellationTokenSource();
        var products = Enumerable.Range(0, 101).Select(i => Product($"Product {i}")).ToList();
        var expectedException = new InvalidOperationException("upsert failed");
        var bulkScope = Substitute.For<IBulkUpsertScope>();
        var productRepo = Substitute.For<IProductRepository>();
        var provider = Substitute.For<IShopProductProvider>();
        var service = CreateService(productRepo, provider, interval: TimeSpan.Zero);
        productRepo
            .BeginBulkUpsertAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(bulkScope));
        provider
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(callInfo =>
                ToAsyncEnumerable(products, cts.Cancel, callInfo.Arg<CancellationToken>())
            );
        bulkScope
            .UpsertAsync(Arg.Any<IReadOnlyCollection<Product>>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw expectedException);

        var actual = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ExecuteForTestAsync(cts.Token)
        );

        Assert.Same(expectedException, actual);
        await productRepo.Received(1).BeginBulkUpsertAsync(Arg.Any<CancellationToken>());
        await bulkScope.Received(1).RollbackAsync();
        await bulkScope.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    private static TestableRepositoryFreshUpService CreateService(
        IProductRepository productRepo,
        IShopProductProvider provider,
        TimeSpan interval
    )
    {
        var services = new ServiceCollection();
        services.AddSingleton(productRepo);
        services.AddSingleton(provider);
        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = Substitute.For<IOptionsMonitor<RepositoryFreshUpServiceOptions>>();
        optionsMonitor.CurrentValue.Returns(
            new RepositoryFreshUpServiceOptions { Interval = interval }
        );
        var logger = Substitute.For<ILogger<RepositoryFreshUpService>>();

        return new TestableRepositoryFreshUpService(serviceProvider, optionsMonitor, logger);
    }

    private static async IAsyncEnumerable<Product> ToAsyncEnumerable(
        IEnumerable<Product> products,
        Action onCompleted,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        foreach (var product in products)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return product;
            await Task.Yield();
        }

        onCompleted();
    }

    private static Product Product(string name) =>
        new(
            Guid.NewGuid(),
            name,
            string.Empty,
            10,
            new Measure(1, MeasureUnit.Count),
            new Uri("https://example.com/product"),
            new Uri("https://example.com/image"),
            Shop.Fozzy
        );

    private sealed class TestableRepositoryFreshUpService(
        IServiceProvider serviceProvider,
        IOptionsMonitor<RepositoryFreshUpServiceOptions> optionsMonitor,
        ILogger<RepositoryFreshUpService> logger
    ) : RepositoryFreshUpService(serviceProvider, optionsMonitor, logger)
    {
        public Task ExecuteForTestAsync(CancellationToken cancellationToken) =>
            ExecuteAsync(cancellationToken);
    }
}
