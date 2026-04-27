using System.Linq.Expressions;
using ApplicationCore;
using ApplicationCore.DTOs;
using ApplicationCore.Entities.Request;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.DevTools.V129.Storage;
using CoreProduct = ApplicationCore.Entities.Product.Product;

namespace Infrastructure.Repository;

internal class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    /// <summary>
    /// Other repo methods could be used, but they won't see non commit changes
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<IBulkUpsertScope> BeginBulkUpsertAsync(CancellationToken ct) =>
        await BulkUpsertScope.CreateNewAsync(dbContext, ct);

    public async Task<PageResultDto<CoreProduct>> FindPageByQueryAsync(
        ProductPageQueryDto query,
        CancellationToken cancellationToken
    )
    {
        var queriable = dbContext.Products.AsQueryable();
        if (query.Request.ApplySearchString)
            queriable = queriable.Where(e =>
                EF.Functions.ILike(e.Name, query.Request.SearchString) // TODO: improve searching
            );

        queriable = query.Request.SortBy switch
        {
            SortBy.Name => queriable.OrderBy(e => e.Name, query.Request.SortOrder),
            SortBy.Price => queriable.OrderBy(e => e.Price, query.Request.SortOrder),
            SortBy.UnifiedPrice => queriable.OrderBy(e => e.UnifiedPrice, query.Request.SortOrder),
            _ => throw new NotImplementedException(
                $"Column to sort by is not defined: '{query.Request.SortBy}'"
            ),
        };

        int total = await queriable.CountAsync(cancellationToken);
        queriable = queriable.Skip(query.Skip).Take(query.Take);
        var items = await queriable.ToListAsync(cancellationToken);

        var coreItems = items.Select(e => e.ToCoreProduct()).ToArray();
        return new PageResultDto<CoreProduct>(coreItems, total);
    }
}

file static class Extensions
{
    public static IQueryable<TSource> OrderBy<TSource, TKey>(
        this IQueryable<TSource> queryable,
        Expression<Func<TSource, TKey>> expr,
        SortOrder sortOrder
    ) =>
        sortOrder switch
        {
            SortOrder.Asc => queryable.OrderBy(expr),
            SortOrder.Desc => queryable.OrderByDescending(expr),
            _ => throw new NotImplementedException($"Ordering is not defined for: '{sortOrder}'"),
        };
}
