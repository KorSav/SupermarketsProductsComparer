using System.Collections.Concurrent;
using ApplicationCore.Entities.Product;
using WebApp.Models;

namespace WebApp.Services;

public interface IProductListService
{
    Task<ProductListViewModel> GetCurrentAsync(Guid userId, CancellationToken cancellationToken);

    Task<ProductListViewModel> RemoveEntryAsync(
        Guid userId,
        Guid entryId,
        CancellationToken cancellationToken
    );

    Task<ProductListViewModel> UpdateEntryAmountAsync(
        Guid userId,
        Guid entryId,
        decimal amount,
        CancellationToken cancellationToken
    );

    Task<Guid> StoreCurrentAsPurchaseAsync(Guid userId, CancellationToken cancellationToken);
}

public sealed class InMemoryProductListService : IProductListService
{
    private readonly InMemoryPurchaseStore _purchaseStore;

    private readonly ConcurrentDictionary<Guid, List<ProductListEntryViewModel>> _lists = new();

    public InMemoryProductListService(InMemoryPurchaseStore purchaseStore)
    {
        _purchaseStore = purchaseStore;
    }

    public Task<ProductListViewModel> GetCurrentAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        List<ProductListEntryViewModel> entries = GetOrCreateUserList(userId);

        ProductListViewModel model = new(entries.ToList());

        return Task.FromResult(model);
    }

    public Task<ProductListViewModel> RemoveEntryAsync(
        Guid userId,
        Guid entryId,
        CancellationToken cancellationToken
    )
    {
        List<ProductListEntryViewModel> entries = GetOrCreateUserList(userId);

        lock (entries)
        {
            ProductListEntryViewModel? entry = entries.FirstOrDefault(x => x.EntryId == entryId);

            if (entry is null)
                throw new InvalidOperationException("Product list entry was not found.");

            entries.Remove(entry);

            return Task.FromResult(new ProductListViewModel(entries.ToList()));
        }
    }

    public Task<ProductListViewModel> UpdateEntryAmountAsync(
        Guid userId,
        Guid entryId,
        decimal amount,
        CancellationToken cancellationToken
    )
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Amount must be greater than zero."
            );

        List<ProductListEntryViewModel> entries = GetOrCreateUserList(userId);

        lock (entries)
        {
            int index = entries.FindIndex(x => x.EntryId == entryId);

            if (index < 0)
                throw new InvalidOperationException("Product list entry was not found.");

            ProductListEntryViewModel current = entries[index];

            entries[index] = current with { Amount = amount };

            return Task.FromResult(new ProductListViewModel(entries.ToList()));
        }
    }

    public Task<Guid> StoreCurrentAsPurchaseAsync(Guid userId, CancellationToken cancellationToken)
    {
        List<ProductListEntryViewModel> entries = GetOrCreateUserList(userId);

        lock (entries)
        {
            if (entries.Count == 0)
                throw new InvalidOperationException("Product list is empty.");

            Guid purchaseId = Guid.NewGuid();

            DemoPurchase purchase = new(
                Id: purchaseId,
                UserId: userId,
                Date: DateTimeOffset.Now,
                Receipt: entries.Select(ToReceiptEntry).ToList()
            );

            _purchaseStore.AddPurchase(purchase);

            entries.Clear();

            return Task.FromResult(purchaseId);
        }
    }

    private static DemoReceiptEntry ToReceiptEntry(ProductListEntryViewModel entry)
    {
        decimal purchasedMeasureCount = entry.Product.Measure.Count * entry.Amount;
        decimal spentAmount = entry.Product.Price * entry.Amount;

        return new DemoReceiptEntry(
            ProductName: entry.Product.Name,
            Measure: new Measure(purchasedMeasureCount, entry.Product.Measure.Unit),
            SpentAmount: spentAmount,
            Shop: entry.Product.Shop
        );
    }

    private List<ProductListEntryViewModel> GetOrCreateUserList(Guid userId)
    {
        return _lists.GetOrAdd(userId, _ => CreateSeedList());
    }

    private static List<ProductListEntryViewModel> CreateSeedList()
    {
        return
        [
            new ProductListEntryViewModel(
                EntryId: Guid.NewGuid(),
                Product: new Product(
                    Name: "Молоко Яготинське 2.6%",
                    Price: 42.50m,
                    Measure: new Measure(1m, MeasureUnit.Litre),
                    LinkProduct: new Uri(
                        "https://silpo.ua/product/moloko-ultrapasteryzovane-selianske-simeine-osoblyve-2-5-726270"
                    ),
                    LinkImage: new Uri(
                        "https://images.silpo.ua/v2/products/300x300/webp/970762f5-ed06-49b4-bbf7-3f3545304283.png"
                    ),
                    Shop: Shop.Silpo
                ),
                Amount: 2m
            ),
            new ProductListEntryViewModel(
                EntryId: Guid.NewGuid(),
                Product: new Product(
                    Name: "Хліб пшеничний",
                    Price: 28.90m,
                    Measure: new Measure(1m, MeasureUnit.Count),
                    LinkProduct: new Uri(
                        "https://fora.ua/product/khlib-kyivkhlib-pshenychnyi-narizanyi-959556"
                    ),
                    LinkImage: new Uri(
                        "https://content.fora.ua/sku/ecommerce/95/480x480wwm/959556_480x480wwm_f67cb10b-287a-f3d9-bf51-d65a7336e09e.png"
                    ),
                    Shop: Shop.Fora
                ),
                Amount: 1m
            ),
            new ProductListEntryViewModel(
                EntryId: Guid.NewGuid(),
                Product: new Product(
                    Name: "Сир кисломолочний",
                    Price: 79.99m,
                    Measure: new Measure(300m, MeasureUnit.Gram),
                    LinkProduct: new Uri(
                        "https://fozzyshop.ua/syr-kyslomolochnyy/1009618-syr-kyslomolochnyi-bilo-5.html"
                    ),
                    LinkImage: new Uri(
                        "https://media.fozzyshop.ua/sku/1009618/product/d3f67dfd-e0c8-7cdb-5138-deced9771678.webp"
                    ),
                    Shop: Shop.Silpo
                ),
                Amount: 1.5m
            ),
        ];
    }
}
