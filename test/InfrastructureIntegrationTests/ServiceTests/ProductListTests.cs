using ApplicationCore;
using ApplicationCore.Entities.Product;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Repository;
using Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Models;
using WebApp.Services;
using static InfrastructureIntegrationTests.Products;

namespace InfrastructureIntegrationTests.ServiceTests;

[Collection(Collections.Container1)]
public class ProductListServiceTests(DbContainerFixture _) : DbPerTestCaseBase(_)
{
    [Fact]
    public async Task AddEntryAsync_CreatesList_AddsNewEntries_MergesExistingEntry_AndReturnsEntries()
    {
        // Arrange
        await PopulateDbAsync();

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var service = new EfProductListService(dbContext);

        Guid userId = await CreateUserAsync(dbContext);

        EfProduct fozzyApple = await FindProductAsync(dbContext, FozzyApple, CancellationToken);
        EfProduct silpoMilk = await FindProductAsync(dbContext, SilpoMilk, CancellationToken);

        // Act
        await service.AddEntryAsync(userId, silpoMilk.Id, 1, CancellationToken);
        await service.AddEntryAsync(userId, fozzyApple.Id, 2, CancellationToken);

        ProductListViewModel act = await service.AddEntryAsync(
            userId,
            fozzyApple.Id,
            3,
            CancellationToken
        );

        // Assert
        act.Entries.Should().HaveCount(2);
        act.Entries.Single(x => x.Product.Id == fozzyApple.Id).Amount.Should().Be(5);
        act.Entries.Single(x => x.Product.Id == silpoMilk.Id).Amount.Should().Be(1);

        EfProductList dbList = await dbContext
            .ProductLists.Include(x => x.Entries)
            .SingleAsync(x => x.UserId == userId, CancellationToken);

        dbList.Entries.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateEntryAmountAsync_And_RemoveEntryAsync_WorkOnlyForCurrentUserEntries()
    {
        // Arrange
        await PopulateDbAsync();

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var service = new EfProductListService(dbContext);

        Guid ownerUserId = await CreateUserAsync(dbContext);
        Guid otherUserId = await CreateUserAsync(dbContext);

        EfProduct silpoMilk = await FindProductAsync(dbContext, SilpoMilk, CancellationToken);
        EfProduct foraCheese = await FindProductAsync(dbContext, ForaCheese, CancellationToken);

        ProductListViewModel ownerList = await service.AddEntryAsync(
            ownerUserId,
            silpoMilk.Id,
            2,
            CancellationToken
        );

        Guid ownerEntryId = ownerList.Entries.Single().EntryId;

        await service.AddEntryAsync(otherUserId, foraCheese.Id, 1, CancellationToken);

        // Act + Assert: another user cannot update owner's entry
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.UpdateEntryAmountAsync(otherUserId, ownerEntryId, 10, CancellationToken)
        );

        // Act
        ProductListViewModel updated = await service.UpdateEntryAmountAsync(
            ownerUserId,
            ownerEntryId,
            4,
            CancellationToken
        );

        // Assert
        updated.Entries.Should().ContainSingle();
        updated.Entries.Single().Amount.Should().Be(4);

        // Act + Assert: another user cannot remove owner's entry
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RemoveEntryAsync(otherUserId, ownerEntryId, CancellationToken)
        );

        // Act
        ProductListViewModel ownerAfterRemove = await service.RemoveEntryAsync(
            ownerUserId,
            ownerEntryId,
            CancellationToken
        );

        // Assert
        ownerAfterRemove.Entries.Should().BeEmpty();

        ProductListViewModel otherUserList = await service.GetCurrentAsync(
            otherUserId,
            CancellationToken
        );

        otherUserList.Entries.Should().ContainSingle();
        otherUserList.Entries.Single().Product.Id.Should().Be(foraCheese.Id);
    }

    [Fact]
    public async Task StoreCurrentAsPurchaseAsync_CreatesPurchaseSnapshot_CalculatesTotal_AndClearsCurrentList()
    {
        // Arrange
        await PopulateDbAsync();

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var service = new EfProductListService(dbContext);

        Guid userId = await CreateUserAsync(dbContext);

        EfProduct fozzyApple = await FindProductAsync(dbContext, FozzyApple, CancellationToken);
        EfProduct silpoMilk = await FindProductAsync(dbContext, SilpoMilk, CancellationToken);

        await service.AddEntryAsync(userId, fozzyApple.Id, 2, CancellationToken);
        await service.AddEntryAsync(userId, silpoMilk.Id, 3, CancellationToken);

        decimal fozzyApplePrice = fozzyApple
            .PriceHistory.OrderByDescending(x => x.ParsedAt)
            .Select(x => x.Price)
            .First();

        decimal silpoMilkPrice = silpoMilk
            .PriceHistory.OrderByDescending(x => x.ParsedAt)
            .Select(x => x.Price)
            .First();

        decimal expectedTotal = fozzyApplePrice * 2 + silpoMilkPrice * 3;

        DateTime before = DateTime.UtcNow;

        // Act
        Guid purchaseId = await service.StoreCurrentAsPurchaseAsync(userId, CancellationToken);

        DateTime after = DateTime.UtcNow;

        // Assert
        EfPurchase purchase = await dbContext
            .Purchases.AsNoTracking()
            .Include(x => x.Entries)
            .SingleAsync(x => x.Id == purchaseId, CancellationToken);

        purchase.UserId.Should().Be(userId);
        purchase.Total.Should().Be(expectedTotal);
        purchase.Date.Should().BeOnOrAfter(before);
        purchase.Date.Should().BeOnOrBefore(after);
        purchase.Entries.Should().HaveCount(2);

        EfPurchaseEntry appleEntry = purchase.Entries.Single(x => x.ProductId == fozzyApple.Id);
        appleEntry.ProductName.Should().Be(FozzyApple.DisplayName);
        appleEntry.MeasureCount.Should().Be(FozzyApple.Measure.Count * 2);
        appleEntry.MeasureUnit.Should().Be(FozzyApple.Measure.Unit);
        appleEntry.SpentAmount.Should().Be(fozzyApplePrice * 2);
        appleEntry.Shop.Should().Be(FozzyApple.Shop);

        EfPurchaseEntry milkEntry = purchase.Entries.Single(x => x.ProductId == silpoMilk.Id);
        milkEntry.ProductName.Should().Be(SilpoMilk.DisplayName);
        milkEntry.MeasureCount.Should().Be(SilpoMilk.Measure.Count * 3);
        milkEntry.MeasureUnit.Should().Be(SilpoMilk.Measure.Unit);
        milkEntry.SpentAmount.Should().Be(silpoMilkPrice * 3);
        milkEntry.Shop.Should().Be(SilpoMilk.Shop);

        ProductListViewModel currentList = await service.GetCurrentAsync(userId, CancellationToken);
        currentList.Entries.Should().BeEmpty();

        int remainingEntriesCount = await dbContext.ProductListEntries.CountAsync(
            x => x.ProductList.UserId == userId,
            CancellationToken
        );

        remainingEntriesCount.Should().Be(0);
    }

    [Fact]
    public async Task Methods_Throw_WhenInputOrStateIsInvalid()
    {
        // Arrange
        await PopulateDbAsync();

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var service = new EfProductListService(dbContext);

        Guid userId = await CreateUserAsync(dbContext);
        Guid userWithEmptyListId = await CreateUserAsync(dbContext);

        EfProduct fozzyApple = await FindProductAsync(dbContext, FozzyApple, CancellationToken);

        dbContext.ProductLists.Add(
            new EfProductList { Id = Guid.NewGuid(), UserId = userWithEmptyListId }
        );

        await dbContext.SaveChangesAsync(CancellationToken);

        Guid missingProductId = Guid.NewGuid();
        Guid missingEntryId = Guid.NewGuid();

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.AddEntryAsync(userId, fozzyApple.Id, 0, CancellationToken)
        );

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.AddEntryAsync(userId, missingProductId, 1, CancellationToken)
        );

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.UpdateEntryAmountAsync(userId, missingEntryId, 0, CancellationToken)
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.UpdateEntryAmountAsync(userId, missingEntryId, 1, CancellationToken)
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RemoveEntryAsync(userId, missingEntryId, CancellationToken)
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.StoreCurrentAsPurchaseAsync(userId, CancellationToken)
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.StoreCurrentAsPurchaseAsync(userWithEmptyListId, CancellationToken)
        );
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

    private async Task<Guid> CreateUserAsync(AppDbContext dbContext)
    {
        Guid id = Guid.NewGuid();

        dbContext
            .Set<EfUser>()
            .Add(
                new EfUser
                {
                    Id = id,
                    Name = $"Test User {id}",
                    Email = $"test-{id}@example.com",
                    PasswordHash = "test-password-hash",
                }
            );

        await dbContext.SaveChangesAsync(CancellationToken);

        return id;
    }

    private static async Task<EfProduct> FindProductAsync(
        AppDbContext dbContext,
        Product product,
        CancellationToken cancellationToken
    )
    {
        return await dbContext
            .Products.Include(x => x.PriceHistory)
            .SingleAsync(
                x =>
                    x.Name == product.Name
                    && x.NameSuffix == product.NameSuffix
                    && x.Shop == product.Shop
                    && x.Amount == product.Measure.Count
                    && x.Unit == product.Measure.Unit,
                cancellationToken
            );
    }

    private static List<Product> All =>
        [ForaCheese, ForaCheese2, SilpoMilk, SilpoDrink, FozzyApple, FozzySpice, FozzySpiceDup];
}
