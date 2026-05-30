using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;
using FluentAssertions;
using Infrastructure.Repository;
using Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Models;
using WebApp.Services;

namespace InfrastructureIntegrationTests.ServiceTests;

[Collection(Collections.Container1)]
public class PurchasesServiceTests(DbContainerFixture _) : DbPerTestCaseBase(_)
{
    [Fact]
    public async Task FindPageAsync_FiltersByUser_NormalizesRanges_SortsByPrice_Paginates_AndMapsReceipt()
    {
        // Arrange
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var service = new EfPurchasesService(dbContext);

        Guid userId = await CreateUserAsync(dbContext);
        Guid otherUserId = await CreateUserAsync(dbContext);

        DateTime jan10 = new(2026, 1, 10, 12, 0, 0, DateTimeKind.Utc);
        DateTime jan15 = new(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        DateTime jan20 = new(2026, 1, 20, 12, 0, 0, DateTimeKind.Utc);
        DateTime feb01 = new(2026, 2, 1, 12, 0, 0, DateTimeKind.Utc);

        Guid cheapPurchaseId = await CreatePurchaseAsync(
            dbContext,
            userId,
            jan10,
            50,
            [
                Entry("Banana", 2, MeasureUnit.KiloGram, 30, Shop.Fozzy),
                Entry("Apple", 1, MeasureUnit.KiloGram, 20, Shop.Silpo),
            ]
        );

        Guid middlePurchaseId = await CreatePurchaseAsync(
            dbContext,
            userId,
            jan15,
            100,
            [Entry("Milk", 2, MeasureUnit.Litre, 100, Shop.Silpo)]
        );

        Guid expensivePurchaseId = await CreatePurchaseAsync(
            dbContext,
            userId,
            jan20,
            200,
            [Entry("Cheese", 1, MeasureUnit.KiloGram, 200, Shop.Fora)]
        );

        await CreatePurchaseAsync(
            dbContext,
            userId,
            feb01,
            500,
            [Entry("Outside date range", 1, MeasureUnit.Count, 500, Shop.Fora)]
        );

        await CreatePurchaseAsync(
            dbContext,
            otherUserId,
            jan15,
            75,
            [Entry("Other user product", 1, MeasureUnit.Count, 75, Shop.Fora)]
        );

        var query = new PurchasesQuery
        {
            // Intentionally reversed. Service should normalize it.
            DateFrom = DateOnly.FromDateTime(jan20),
            DateTo = DateOnly.FromDateTime(jan10),

            // Intentionally reversed. Service should normalize it.
            MinTotal = 200,
            MaxTotal = 50,

            SortBy = PurchaseSortBy.Price,
            SortOrder = SortOrder.Asc,

            Page = 2,
            PageSize = 2,
        };

        // Act
        var act = await service.FindPageAsync(userId, query, CancellationToken);

        // Assert
        act.TotalItems.Should().Be(3);
        act.PageNo.Should().Be(1);
        act.PageSize.Should().Be(2);
        act.PagesCount.Should().Be(2);

        act.Should().ContainSingle();

        PurchaseListItemViewModel item = act.Single();

        item.Id.Should().Be(expensivePurchaseId);
        item.Total.Should().Be(200);
        item.Date.Should().Be(jan20);

        item.Receipt.Should().ContainSingle();
        item.Receipt.Single().ProductName.Should().Be("Cheese");
        item.Receipt.Single().Measure.Should().Be(new Measure(1, MeasureUnit.KiloGram));
        item.Receipt.Single().SpentAmount.Should().Be(200);
        item.Receipt.Single().Shop.Should().Be(Shop.Fora);

        act.Select(x => x.Id).Should().NotContain(cheapPurchaseId);
        act.Select(x => x.Id).Should().NotContain(middlePurchaseId);
    }

    [Fact]
    public async Task FindPageAsync_NormalizesInvalidPagingAndNegativeTotals_AndSortsByDateDescending()
    {
        // Arrange
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var service = new EfPurchasesService(dbContext);

        Guid userId = await CreateUserAsync(dbContext);

        DateTime olderDate = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        DateTime newerDate = new(2026, 1, 2, 12, 0, 0, DateTimeKind.Utc);

        Guid olderPurchaseId = await CreatePurchaseAsync(
            dbContext,
            userId,
            olderDate,
            10,
            [Entry("Older", 1, MeasureUnit.Count, 10, Shop.Fora)]
        );

        Guid newerPurchaseId = await CreatePurchaseAsync(
            dbContext,
            userId,
            newerDate,
            20,
            [Entry("Newer", 1, MeasureUnit.Count, 20, Shop.Silpo)]
        );

        var query = new PurchasesQuery
        {
            Page = -5,
            PageSize = -1,
            MinTotal = -100,
            MaxTotal = -1,
            SortBy = PurchaseSortBy.Date,
            SortOrder = SortOrder.Desc,
        };

        // Act
        var act = await service.FindPageAsync(userId, query, CancellationToken);

        // Assert
        act.TotalItems.Should().Be(2);
        act.PageNo.Should().Be(0);
        act.PageSize.Should().Be(10);

        act.Select(x => x.Id).Should().Equal(newerPurchaseId, olderPurchaseId);
    }

    [Fact]
    public async Task RemoveAsync_RemovesOwnPurchaseWithReceipt_AndRejectsMissingOrOtherUsersPurchase()
    {
        // Arrange
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var service = new EfPurchasesService(dbContext);

        Guid userId = await CreateUserAsync(dbContext);
        Guid otherUserId = await CreateUserAsync(dbContext);

        Guid ownPurchaseId = await CreatePurchaseAsync(
            dbContext,
            userId,
            new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            100,
            [
                Entry("Milk", 2, MeasureUnit.Litre, 60, Shop.Silpo),
                Entry("Bread", 1, MeasureUnit.Count, 40, Shop.Fora),
            ]
        );

        Guid otherUsersPurchaseId = await CreatePurchaseAsync(
            dbContext,
            otherUserId,
            new DateTime(2026, 1, 2, 12, 0, 0, DateTimeKind.Utc),
            200,
            [Entry("Cheese", 1, MeasureUnit.KiloGram, 200, Shop.Fora)]
        );

        // Act + Assert: current user cannot remove another user's purchase
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RemoveAsync(userId, otherUsersPurchaseId, CancellationToken)
        );

        // Act
        await service.RemoveAsync(userId, ownPurchaseId, CancellationToken);

        // Assert
        bool ownPurchaseExists = await dbContext.Purchases.AnyAsync(
            x => x.Id == ownPurchaseId,
            CancellationToken
        );

        ownPurchaseExists.Should().BeFalse();

        int ownPurchaseReceiptEntriesCount = await dbContext
            .Set<EfPurchaseEntry>()
            .CountAsync(x => x.PurchaseId == ownPurchaseId, CancellationToken);

        ownPurchaseReceiptEntriesCount.Should().Be(0);

        bool otherUsersPurchaseExists = await dbContext.Purchases.AnyAsync(
            x => x.Id == otherUsersPurchaseId,
            CancellationToken
        );

        otherUsersPurchaseExists.Should().BeTrue();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RemoveAsync(userId, Guid.NewGuid(), CancellationToken)
        );
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

    private async Task<Guid> CreatePurchaseAsync(
        AppDbContext dbContext,
        Guid userId,
        DateTime date,
        decimal total,
        IReadOnlyCollection<EfPurchaseEntry> entries
    )
    {
        Guid purchaseId = Guid.NewGuid();

        var purchase = new EfPurchase
        {
            Id = purchaseId,
            UserId = userId,
            Date = date,
            Total = total,
            Entries = entries.ToList(),
        };

        foreach (EfPurchaseEntry entry in purchase.Entries)
        {
            entry.Id = Guid.NewGuid();
            entry.PurchaseId = purchaseId;
        }

        dbContext.Purchases.Add(purchase);

        await dbContext.SaveChangesAsync(CancellationToken);

        return purchaseId;
    }

    private static EfPurchaseEntry Entry(
        string productName,
        decimal measureCount,
        MeasureUnit measureUnit,
        decimal spentAmount,
        Shop shop
    )
    {
        return new EfPurchaseEntry
        {
            ProductName = productName,
            MeasureCount = measureCount,
            MeasureUnit = measureUnit,
            SpentAmount = spentAmount,
            Shop = shop,
        };
    }
}
