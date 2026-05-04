using System.Collections.Concurrent;
using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;
using WebApp.Models;

namespace WebApp.Services;

public sealed record DemoReceiptEntry(
    string ProductName,
    Measure Measure,
    decimal SpentAmount,
    Shop Shop
);

public sealed record DemoPurchase(
    Guid Id,
    Guid UserId,
    DateTimeOffset Date,
    IReadOnlyList<DemoReceiptEntry> Receipt
)
{
    public decimal Total => Receipt.Sum(x => x.SpentAmount);
}

public sealed class InMemoryPurchaseStore
{
    private readonly ConcurrentDictionary<Guid, List<DemoPurchase>> _purchasesByUser = new();

    public IReadOnlyList<DemoPurchase> GetUserPurchases(Guid userId)
    {
        List<DemoPurchase> purchases = GetOrCreateUserPurchases(userId);

        lock (purchases)
        {
            return purchases.ToList();
        }
    }

    public void AddPurchase(DemoPurchase purchase)
    {
        List<DemoPurchase> purchases = GetOrCreateUserPurchases(purchase.UserId);

        lock (purchases)
        {
            purchases.Add(purchase);
        }
    }

    public bool RemovePurchase(Guid userId, Guid purchaseId)
    {
        List<DemoPurchase> purchases = GetOrCreateUserPurchases(userId);

        lock (purchases)
        {
            DemoPurchase? purchase = purchases.FirstOrDefault(x => x.Id == purchaseId);

            if (purchase is null)
                return false;

            purchases.Remove(purchase);
            return true;
        }
    }

    private List<DemoPurchase> GetOrCreateUserPurchases(Guid userId)
    {
        return _purchasesByUser.GetOrAdd(userId, CreateSeedPurchases);
    }

    private static List<DemoPurchase> CreateSeedPurchases(Guid userId)
    {
        return
        [
            new DemoPurchase(
                Id: Guid.NewGuid(),
                UserId: userId,
                Date: DateTimeOffset.Now.AddDays(-1),
                Receipt:
                [
                    new DemoReceiptEntry(
                        ProductName: "Молоко Яготинське 2.6%",
                        Measure: new Measure(2m, MeasureUnit.Litre),
                        SpentAmount: 85.00m,
                        Shop: Shop.Silpo
                    ),
                    new DemoReceiptEntry(
                        ProductName: "Хліб пшеничний",
                        Measure: new Measure(1m, MeasureUnit.Count),
                        SpentAmount: 28.90m,
                        Shop: Shop.Fozzy
                    ),
                ]
            ),
            new DemoPurchase(
                Id: Guid.NewGuid(),
                UserId: userId,
                Date: DateTimeOffset.Now.AddDays(-4),
                Receipt:
                [
                    new DemoReceiptEntry(
                        ProductName: "Сир кисломолочний",
                        Measure: new Measure(600m, MeasureUnit.Gram),
                        SpentAmount: 159.98m,
                        Shop: Shop.Silpo
                    ),
                    new DemoReceiptEntry(
                        ProductName: "Яйця курячі",
                        Measure: new Measure(10m, MeasureUnit.Count),
                        SpentAmount: 64.50m,
                        Shop: Shop.Silpo
                    ),
                ]
            ),
            new DemoPurchase(
                Id: Guid.NewGuid(),
                UserId: userId,
                Date: DateTimeOffset.Now.AddDays(-10),
                Receipt:
                [
                    new DemoReceiptEntry(
                        ProductName: "Кава мелена",
                        Measure: new Measure(250m, MeasureUnit.Gram),
                        SpentAmount: 189.99m,
                        Shop: Shop.Fozzy
                    ),
                ]
            ),
            new DemoPurchase(
                Id: Guid.NewGuid(),
                UserId: userId,
                Date: DateTimeOffset.Now.AddDays(-10),
                Receipt:
                [
                    new DemoReceiptEntry(
                        ProductName: "Кава мелена",
                        Measure: new Measure(250m, MeasureUnit.Gram),
                        SpentAmount: 189.99m,
                        Shop: Shop.Fozzy
                    ),
                ]
            ),
            new DemoPurchase(
                Id: Guid.NewGuid(),
                UserId: userId,
                Date: DateTimeOffset.Now.AddDays(-10),
                Receipt:
                [
                    new DemoReceiptEntry(
                        ProductName: "Кава мелена",
                        Measure: new Measure(250m, MeasureUnit.Gram),
                        SpentAmount: 189.99m,
                        Shop: Shop.Fozzy
                    ),
                ]
            ),
            new DemoPurchase(
                Id: Guid.NewGuid(),
                UserId: userId,
                Date: DateTimeOffset.Now.AddDays(-10),
                Receipt:
                [
                    new DemoReceiptEntry(
                        ProductName: "Кава мелена",
                        Measure: new Measure(250m, MeasureUnit.Gram),
                        SpentAmount: 189.99m,
                        Shop: Shop.Fozzy
                    ),
                ]
            ),
            new DemoPurchase(
                Id: Guid.NewGuid(),
                UserId: userId,
                Date: DateTimeOffset.Now.AddDays(-10),
                Receipt:
                [
                    new DemoReceiptEntry(
                        ProductName: "Кава мелена",
                        Measure: new Measure(250m, MeasureUnit.Gram),
                        SpentAmount: 189.99m,
                        Shop: Shop.Fozzy
                    ),
                ]
            ),
            new DemoPurchase(
                Id: Guid.NewGuid(),
                UserId: userId,
                Date: DateTimeOffset.Now.AddDays(-10),
                Receipt:
                [
                    new DemoReceiptEntry(
                        ProductName: "Кава мелена",
                        Measure: new Measure(250m, MeasureUnit.Gram),
                        SpentAmount: 189.99m,
                        Shop: Shop.Fozzy
                    ),
                ]
            ),
            new DemoPurchase(
                Id: Guid.NewGuid(),
                UserId: userId,
                Date: DateTimeOffset.Now.AddDays(-10),
                Receipt:
                [
                    new DemoReceiptEntry(
                        ProductName: "Кава мелена",
                        Measure: new Measure(250m, MeasureUnit.Gram),
                        SpentAmount: 189.99m,
                        Shop: Shop.Fozzy
                    ),
                ]
            ),
            new DemoPurchase(
                Id: Guid.NewGuid(),
                UserId: userId,
                Date: DateTimeOffset.Now.AddDays(-10),
                Receipt:
                [
                    new DemoReceiptEntry(
                        ProductName: "Кава мелена",
                        Measure: new Measure(250m, MeasureUnit.Gram),
                        SpentAmount: 189.99m,
                        Shop: Shop.Fozzy
                    ),
                ]
            ),
        ];
    }
}

public interface IPurchasesService
{
    Task<PagedResult<PurchaseListItemViewModel>> FindPageAsync(
        Guid userId,
        PurchasesQuery query,
        CancellationToken cancellationToken
    );

    Task RemoveAsync(Guid userId, Guid purchaseId, CancellationToken cancellationToken);
}

public sealed class InMemoryPurchasesService(InMemoryPurchaseStore purchaseStore)
    : IPurchasesService
{
    public Task<PagedResult<PurchaseListItemViewModel>> FindPageAsync(
        Guid userId,
        PurchasesQuery query,
        CancellationToken cancellationToken
    )
    {
        PurchasesQuery normalizedQuery = NormalizeQuery(query);

        IEnumerable<DemoPurchase> purchases = purchaseStore.GetUserPurchases(userId);

        if (normalizedQuery.DateFrom is not null)
        {
            DateOnly dateFrom = normalizedQuery.DateFrom.Value;

            purchases = purchases.Where(x =>
                DateOnly.FromDateTime(x.Date.LocalDateTime) >= dateFrom
            );
        }

        if (normalizedQuery.DateTo is not null)
        {
            DateOnly dateTo = normalizedQuery.DateTo.Value;

            purchases = purchases.Where(x => DateOnly.FromDateTime(x.Date.LocalDateTime) <= dateTo);
        }

        if (normalizedQuery.MinTotal is not null)
        {
            purchases = purchases.Where(x => x.Total >= normalizedQuery.MinTotal.Value);
        }

        if (normalizedQuery.MaxTotal is not null)
        {
            purchases = purchases.Where(x => x.Total <= normalizedQuery.MaxTotal.Value);
        }

        purchases = ApplySorting(purchases, normalizedQuery);

        int totalCount = purchases.Count();

        IReadOnlyList<PurchaseListItemViewModel> items = purchases
            .Skip((normalizedQuery.Page - 1) * normalizedQuery.PageSize)
            .Take(normalizedQuery.PageSize)
            .Select(ToViewModel)
            .ToList();

        PagedResult<PurchaseListItemViewModel> result = new(
            Items: items,
            TotalCount: totalCount,
            Page: normalizedQuery.Page,
            PageSize: normalizedQuery.PageSize
        );

        return Task.FromResult(result);
    }

    public Task RemoveAsync(Guid userId, Guid purchaseId, CancellationToken cancellationToken)
    {
        bool removed = purchaseStore.RemovePurchase(userId, purchaseId);

        if (!removed)
            throw new InvalidOperationException("Purchase was not found.");

        return Task.CompletedTask;
    }

    private static IEnumerable<DemoPurchase> ApplySorting(
        IEnumerable<DemoPurchase> purchases,
        PurchasesQuery query
    )
    {
        return (query.SortBy, query.SortOrder) switch
        {
            (PurchaseSortBy.Price, SortOrder.Asc) => purchases.OrderBy(x => x.Total),
            (PurchaseSortBy.Price, SortOrder.Desc) => purchases.OrderByDescending(x => x.Total),

            (PurchaseSortBy.Date, SortOrder.Asc) => purchases.OrderBy(x => x.Date),
            _ => purchases.OrderByDescending(x => x.Date),
        };
    }

    private static PurchaseListItemViewModel ToViewModel(DemoPurchase purchase)
    {
        return new PurchaseListItemViewModel(
            Id: purchase.Id,
            Date: purchase.Date,
            Total: purchase.Total,
            Receipt: purchase
                .Receipt.Select(x => new ReceiptEntryViewModel(
                    ProductName: x.ProductName,
                    Measure: x.Measure,
                    SpentAmount: x.SpentAmount,
                    Shop: x.Shop
                ))
                .ToList()
        );
    }

    private static PurchasesQuery NormalizeQuery(PurchasesQuery query)
    {
        query.Page = query.Page < 1 ? 1 : query.Page;

        query.PageSize = query.PageSize switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => query.PageSize,
        };

        if (query.MinTotal < 0)
            query.MinTotal = null;

        if (query.MaxTotal < 0)
            query.MaxTotal = null;

        if (query.DateFrom is not null && query.DateTo is not null && query.DateFrom > query.DateTo)
        {
            (query.DateFrom, query.DateTo) = (query.DateTo, query.DateFrom);
        }

        if (
            query.MinTotal is not null
            && query.MaxTotal is not null
            && query.MinTotal > query.MaxTotal
        )
        {
            (query.MinTotal, query.MaxTotal) = (query.MaxTotal, query.MinTotal);
        }

        return query;
    }
}
