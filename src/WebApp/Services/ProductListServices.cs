using System.Data;
using ApplicationCore.Entities.Product;
using Infrastructure.Repository;
using Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Services;

public interface IProductListService
{
    Task<ProductListViewModel> GetCurrentAsync(Guid userId, CancellationToken cancellationToken);

    Task<ProductListViewModel> AddEntryAsync(
        Guid userId,
        Guid productId,
        decimal amount,
        CancellationToken cancellationToken
    );

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

public sealed class EfProductListService(AppDbContext dbContext) : IProductListService
{
    public async Task<ProductListViewModel> GetCurrentAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        EfProductList? list = await dbContext
            .ProductLists.AsNoTracking()
            .Include(x => x.Entries)
            .ThenInclude(x => x.Product)
            .ThenInclude(x => x.PriceHistory)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (list is null)
        {
            return new ProductListViewModel([]);
        }

        return ToProductListViewModel(list);
    }

    public async Task<ProductListViewModel> AddEntryAsync(
        Guid userId,
        Guid productId,
        decimal amount,
        CancellationToken cancellationToken
    )
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Amount must be greater than zero."
            );

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken
        );

        EfProductList list = await GetOrCreateCurrentListAsync(userId, cancellationToken);

        bool productExists = await dbContext.Products.AnyAsync(
            x => x.Id == productId,
            cancellationToken
        );

        if (!productExists)
            throw new KeyNotFoundException("Product was not found.");

        EfProductListEntry? existingEntry = await dbContext.ProductListEntries.FirstOrDefaultAsync(
            x => x.ProductListId == list.Id && x.ProductId == productId,
            cancellationToken
        );

        if (existingEntry is not null)
        {
            existingEntry.Amount += amount;
        }
        else
        {
            dbContext.ProductListEntries.Add(
                new EfProductListEntry
                {
                    Id = Guid.NewGuid(),
                    ProductListId = list.Id,
                    ProductId = productId,
                    Amount = amount,
                }
            );
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await GetCurrentAsync(userId, cancellationToken);
    }

    public async Task<ProductListViewModel> RemoveEntryAsync(
        Guid userId,
        Guid entryId,
        CancellationToken cancellationToken
    )
    {
        EfProductListEntry? entry = await dbContext
            .ProductListEntries.Include(x => x.ProductList)
            .FirstOrDefaultAsync(
                x => x.Id == entryId && x.ProductList.UserId == userId,
                cancellationToken
            );

        if (entry is null)
            throw new InvalidOperationException("Product list entry was not found.");

        dbContext.ProductListEntries.Remove(entry);

        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetCurrentAsync(userId, cancellationToken);
    }

    public async Task<ProductListViewModel> UpdateEntryAmountAsync(
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

        EfProductListEntry? entry = await dbContext
            .ProductListEntries.Include(x => x.ProductList)
            .FirstOrDefaultAsync(
                x => x.Id == entryId && x.ProductList.UserId == userId,
                cancellationToken
            );

        if (entry is null)
            throw new InvalidOperationException("Product list entry was not found.");

        entry.Amount = amount;

        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetCurrentAsync(userId, cancellationToken);
    }

    public async Task<Guid> StoreCurrentAsPurchaseAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken
        );

        EfProductList list =
            await dbContext
                .ProductLists.Include(x => x.Entries)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.PriceHistory)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken)
            ?? throw new InvalidOperationException("Product list was not found.");

        if (list.Entries.Count == 0)
            throw new InvalidOperationException("Product list is empty.");

        List<EfPurchaseEntry> purchaseEntries = list.Entries.Select(ToPurchaseEntry).ToList();

        Guid purchaseId = Guid.NewGuid();

        foreach (EfPurchaseEntry entry in purchaseEntries)
        {
            entry.PurchaseId = purchaseId;
        }

        EfPurchase purchase = new()
        {
            Id = purchaseId,
            UserId = userId,
            Date = DateTime.UtcNow,
            Entries = purchaseEntries,
            Total = purchaseEntries.Sum(x => x.SpentAmount),
        };

        dbContext.Purchases.Add(purchase);
        dbContext.ProductListEntries.RemoveRange(list.Entries);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return purchaseId;
    }

    private async Task<EfProductList> GetOrCreateCurrentListAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        EfProductList? existingList = await dbContext.ProductLists.FirstOrDefaultAsync(
            x => x.UserId == userId,
            cancellationToken
        );

        if (existingList is not null)
            return existingList;

        EfProductList newList = new() { Id = Guid.NewGuid(), UserId = userId };

        dbContext.ProductLists.Add(newList);

        await dbContext.SaveChangesAsync(cancellationToken);

        return newList;
    }

    private static ProductListViewModel ToProductListViewModel(EfProductList list)
    {
        IReadOnlyList<ProductListEntryViewModel> entries = list
            .Entries.OrderBy(x => x.Product.Name)
            .Select(x => new ProductListEntryViewModel(
                EntryId: x.Id,
                Product: x.Product.ToCoreProduct(),
                Amount: x.Amount
            ))
            .ToList();

        return new ProductListViewModel(entries);
    }

    private static EfPurchaseEntry ToPurchaseEntry(EfProductListEntry entry)
    {
        Product product = entry.Product.ToCoreProduct();

        decimal purchasedMeasureCount = product.Measure.Count * entry.Amount;
        decimal spentAmount = product.Price * entry.Amount;

        return new EfPurchaseEntry
        {
            Id = Guid.NewGuid(),

            // Nullable FK, but we still store it while product exists.
            ProductId = product.Id,

            ProductName = product.Name,
            MeasureCount = purchasedMeasureCount,
            MeasureUnit = product.Measure.Unit,
            SpentAmount = spentAmount,
            Shop = product.Shop,
        };
    }
}
