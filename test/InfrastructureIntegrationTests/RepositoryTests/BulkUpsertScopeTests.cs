using ApplicationCore;
using ApplicationCore.Entities.Product;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfrastructureIntegrationTests.RepositoryTests;

public sealed class BulkUpsertScopeTests(DbContainerFixture _)
    : DbPerTestCaseBase(_),
        IClassFixture<DbContainerFixture>
{
    [Fact]
    public async Task BulkUpsert_InsertsDataAndDropsTempTable()
    {
        // Arrange + Act
        await using (var scope = Services.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var bulkScope = await repo.BeginBulkUpsertAsync(CancellationToken);
            await bulkScope.UpsertAsync(AllProducts(), CancellationToken);
            await bulkScope.CommitAsync(CancellationToken);
        }

        // Assert
        await using (var scope = Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var expected = AllProducts();
            var actual = await dbContext
                .Products.Select(p => p.ToCoreProduct())
                .ToListAsync(CancellationToken);
            Assert.Equal(expected, actual);

            var tempTableExists = await dbContext
                .Database.SqlQueryRaw<bool>(
                    $"SELECT EXISTS(SELECT 1 FROM pg_tables WHERE tablename=\'{BulkUpsertScope.TempTableName}\') AS \"Value\""
                )
                .SingleAsync(CancellationToken);
            Assert.False(tempTableExists);
        }
    }

    [Fact]
    public async Task BulkUpsert_OverwritesExistingData()
    {
        // Arrange
        await using (var scope = Services.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var bulkScope = await repo.BeginBulkUpsertAsync(CancellationToken);
            await bulkScope.UpsertAsync(AllProducts(), CancellationToken);
            await bulkScope.CommitAsync(CancellationToken);
        }

        // Act
        await using (var scope = Services.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var bulkScope = await repo.BeginBulkUpsertAsync(CancellationToken);
            await bulkScope.UpsertAsync(AllProductsModified(), CancellationToken);
            await bulkScope.CommitAsync(CancellationToken);
        }

        // Assert
        await using (var scope = Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var expected = AllProductsModified();
            var actual = await dbContext
                .Products.Select(p => p.ToCoreProduct())
                .ToListAsync(CancellationToken);
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public async Task BulkUpsert_Succeeds_IfSimilarProductsAppearMultipleTimes()
    {
        // Act
        await using (var scope = Services.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var bulkScope = await repo.BeginBulkUpsertAsync(CancellationToken);
            await bulkScope.UpsertAsync(AllProducts(), CancellationToken);
            await bulkScope.UpsertAsync(AllProductsModified(), CancellationToken);
            await bulkScope.CommitAsync(CancellationToken);
        }

        // Assert
        await using (var scope = Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var ps1 = AllProducts();
            var ps2 = AllProductsModified();
            var act = await dbContext
                .Products.Select(p => p.ToCoreProduct())
                .ToListAsync(CancellationToken);
            Assert.Equal(ps1.Count, act.Count); // randomly deduplicates
            foreach (var ((p1, p2), actP) in ps1.Zip(ps2).Zip(act).ToList())
                Assert.Contains([p1, p2], p => p == actP);
        }
    }

    [Fact]
    public async Task Rollback_CancelsUpsertsAndRemovesTempTable()
    {
        // Arrange + Act
        await using (var scope = Services.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            static void codeThrowingException() => throw new Exception();
            var bulkScope = await repo.BeginBulkUpsertAsync(CancellationToken);
            try
            {
                await bulkScope.UpsertAsync(AllProducts(), CancellationToken);
                await bulkScope.UpsertAsync(
                    [FozzyApple with { Name = "Apple t2" }],
                    CancellationToken
                );
                codeThrowingException();
                await bulkScope.CommitAsync(CancellationToken);
            }
            catch
            {
                await bulkScope.RollbackAsync();
            }
        }

        // Assert
        await using (var scope = Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var actual = await dbContext.Products.CountAsync(CancellationToken);
            Assert.Equal(0, actual);

            var tempTableExists = await dbContext
                .Database.SqlQueryRaw<bool>(
                    $"SELECT EXISTS(SELECT 1 FROM pg_tables WHERE tablename=\'{BulkUpsertScope.TempTableName}\') AS \"Value\""
                )
                .SingleAsync(CancellationToken);
            Assert.False(tempTableExists);
        }
    }

    private static List<Product> AllProductsModified() =>
        [
            SilpoMilk with
            {
                Price = 100m,
            },
            FozzyApple with
            {
                LinkProduct = new Uri("https://changed-product"),
                LinkImage = new Uri("https://changed-image"),
            },
            ForaDrink with
            {
                Measure = new(300, MeasureUnit.Gram),
            },
        ];

    private List<Product> AllProducts() => [SilpoMilk, FozzyApple, ForaDrink];

    private static Product SilpoMilk =>
        new(
            Name: "Молоко, 1л",
            Measure: new(1, MeasureUnit.Litre),
            Price: 30,
            LinkImage: new Uri("https://non-existing-images-silpo.com"),
            LinkProduct: new Uri("https://non-existing-products-silpo.com"),
            Shop: Shop.Silpo
        );
    private static Product FozzyApple =>
        new(
            Name: "Apple 🍎, 500г",
            Measure: new(500, MeasureUnit.Gram),
            Price: 15,
            LinkImage: new Uri("https://image-stock.com"),
            LinkProduct: new Uri("https://non-existing-products-fozzy.com"),
            Shop: Shop.Fozzy
        );
    private static Product ForaDrink =>
        new(
            Name: "Напій juice, 200мл",
            Measure: new(200, MeasureUnit.MiliLitre),
            Price: 20,
            LinkImage: new Uri("https://image-stock.com"),
            LinkProduct: new Uri("https://non-existing-products-fora.com"),
            Shop: Shop.Fora
        );
}
