using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;
using ApplicationCore.Utils;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Services;

public interface IPurchasesService
{
    Task<PaginatedList<PurchaseListItemViewModel>> FindPageAsync(
        Guid userId,
        PurchasesQuery query,
        CancellationToken cancellationToken
    );

    Task RemoveAsync(Guid userId, Guid purchaseId, CancellationToken cancellationToken);
}

public sealed class EfPurchasesService(AppDbContext dbContext) : IPurchasesService
{
    public async Task<PaginatedList<PurchaseListItemViewModel>> FindPageAsync(
        Guid userId,
        PurchasesQuery query,
        CancellationToken cancellationToken
    )
    {
        PurchasesQuery normalizedQuery = NormalizeQuery(query);

        IQueryable<Infrastructure.Repository.Entities.EfPurchase> purchases = dbContext
            .Purchases.Include(x => x.Entries)
            .Where(x => x.UserId == userId);

        if (normalizedQuery.DateFrom is not null)
        {
            DateTime dateFrom = normalizedQuery.DateFrom.Value.ToDateTime(TimeOnly.MinValue);
            purchases = purchases.Where(x => x.Date >= dateFrom);
        }

        if (normalizedQuery.DateTo is not null)
        {
            DateTime dateToExclusive = normalizedQuery
                .DateTo.Value.AddDays(1)
                .ToDateTime(TimeOnly.MinValue);

            purchases = purchases.Where(x => x.Date < dateToExclusive);
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

        int totalCount = await purchases.CountAsync(cancellationToken);

        IReadOnlyList<PurchaseListItemViewModel> items = await purchases
            .Skip((normalizedQuery.Page - 1) * normalizedQuery.PageSize)
            .Take(normalizedQuery.PageSize)
            .Select(x => new PurchaseListItemViewModel(
                x.Id,
                x.Date,
                x.Total,
                x.Entries.OrderBy(e => e.ProductName)
                    .Select(e => new ReceiptEntryViewModel(
                        e.ProductName,
                        new Measure(e.MeasureCount, e.MeasureUnit),
                        e.SpentAmount,
                        e.Shop
                    ))
                    .ToList()
            ))
            .ToListAsync(cancellationToken);

        return new PaginatedList<PurchaseListItemViewModel>(
            items,
            totalCount,
            normalizedQuery.Page - 1,
            normalizedQuery.PageSize
        );
    }

    public async Task RemoveAsync(Guid userId, Guid purchaseId, CancellationToken cancellationToken)
    {
        Infrastructure.Repository.Entities.EfPurchase? purchase =
            await dbContext.Purchases.FirstOrDefaultAsync(
                x => x.Id == purchaseId && x.UserId == userId,
                cancellationToken
            );

        if (purchase is null)
            throw new InvalidOperationException("Purchase was not found.");

        dbContext.Purchases.Remove(purchase);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Infrastructure.Repository.Entities.EfPurchase> ApplySorting(
        IQueryable<Infrastructure.Repository.Entities.EfPurchase> purchases,
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
