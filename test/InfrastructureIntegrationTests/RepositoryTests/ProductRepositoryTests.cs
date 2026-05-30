using System.Diagnostics;
using ApplicationCore;
using ApplicationCore.DTOs;
using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static InfrastructureIntegrationTests.Products;

namespace InfrastructureIntegrationTests.RepositoryTests;

[Collection(Collections.Container1)]
public class ProductRepositoryTests(DbContainerFixture _) : DbPerTestCaseBase(_)
{
    private const int _pageLimit = 3;

    [Theory]
    [InlineData(SortOrder.Asc), InlineData(SortOrder.Desc)]
    public async Task FindPageByQueryAsync_OrdersResultByName(SortOrder sortOrder)
    {
        // Arrange + Act
        await PopulateDbAsync();
        await using var scope = Services.CreateAsyncScope();
        var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var query = new ProductPageQueryDto(
            0,
            _pageLimit,
            new Request(" \n\t", SortBy.Name, sortOrder)
        );
        var act = await repo.FindPageByQueryAsync(query, CancellationToken);

        // Assert
        PageResultDto<Product> exp = sortOrder switch
        {
            SortOrder.Asc => new([FozzyApple, FozzySpice, SilpoMilk], 6),
            SortOrder.Desc => new([ForaCheese, ForaCheese2, SilpoDrink], 6),
            _ => throw new UnreachableException(),
        };
        exp.Items.Should().BeEquivalentTo(act.Items, o => o.Excluding(e => e.Id));
        Assert.Equal(exp.Total, act.Total);
    }

    [Theory]
    [InlineData(SortOrder.Asc), InlineData(SortOrder.Desc)]
    public async Task FindPageByQueryAsync_OrdersResultByUnifiedPrice(SortOrder sortOrder)
    {
        // Arrange + Act
        await PopulateDbAsync();
        await using var scope = Services.CreateAsyncScope();
        var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var query = new ProductPageQueryDto(
            0,
            _pageLimit,
            new Request(" гол сир\n ", SortBy.UnifiedPrice, sortOrder)
        );
        var act = await repo.FindPageByQueryAsync(query, CancellationToken);
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Assert
        PageResultDto<Product> exp = sortOrder switch
        {
            SortOrder.Asc => new([ForaCheese2, ForaCheese], 2),
            SortOrder.Desc => new([ForaCheese, ForaCheese2], 2),
            _ => throw new UnreachableException(),
        };
        exp.Items.Should().BeEquivalentTo(act.Items, o => o.Excluding(e => e.Id));
        Assert.Equal(exp.Total, act.Total);
    }

    [Fact]
    public async Task FindPageByQueryAsync_DoesProperPagination()
    {
        await PopulateDbAsync();
        // First page
        await using (var scope = Services.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var query = new ProductPageQueryDto(0, 2, new Request("", SortBy.Price, SortOrder.Asc));
            var act = await repo.FindPageByQueryAsync(query, CancellationToken);
            PageResultDto<Product> exp = new([FozzyApple, FozzySpice], 6);
            exp.Items.Should().BeEquivalentTo(act.Items, o => o.Excluding(e => e.Id));
            Assert.Equal(exp.Total, act.Total);
        }

        // Second page
        await using (var scope = Services.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var query = new ProductPageQueryDto(1, 2, new Request("", SortBy.Price, SortOrder.Asc));
            var act = await repo.FindPageByQueryAsync(query, CancellationToken);
            PageResultDto<Product> exp = new([SilpoMilk, SilpoDrink], 6);
            exp.Items.Should().BeEquivalentTo(act.Items, o => o.Excluding(e => e.Id));
            Assert.Equal(exp.Total, act.Total);
        }
    }

    private async Task PopulateDbAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.ExecuteSqlRawAsync(SqlScripts.BulkMergeProc);
        var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var bulkUpsert = await repo.BeginBulkUpsertAsync(CancellationToken);
        await bulkUpsert.UpsertAsync(All, CancellationToken);
        await bulkUpsert.CommitAsync(CancellationToken);
    }

    private static List<Product> All =>
        [ForaCheese, ForaCheese2, SilpoMilk, SilpoDrink, FozzyApple, FozzySpice, FozzySpiceDup];
}
