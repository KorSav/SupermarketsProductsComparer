using System.Linq.Expressions;
using ApplicationCore;
using ApplicationCore.DTOs;
using ApplicationCore.Entities.Request;
using Microsoft.EntityFrameworkCore;
using CoreProduct = ApplicationCore.Entities.Product.Product;

namespace Infrastructure.Repository;

internal class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    /// <summary>
    /// Other repo methods could be used, but they won't see non committed changes
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
        var queriable = dbContext.Products.Include(e => e.PriceHistory).AsQueryable();
        if (query.Request.ApplySearchString)
        {
            var words = query.Request.SearchString.Split(
                ' ',
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
            );
            foreach (var w in words) // TODO: improve searching
                queriable = queriable.Where(e => EF.Functions.ILike(e.Name, w.ToPattern(), @"\"));
        }

        queriable = query.Request.SortBy switch
        {
            SortBy.Name => queriable.OrderBy(e => e.Name, query.Request.SortOrder),
            SortBy.Price => queriable.OrderBy(
                e =>
                    e.PriceHistory.OrderByDescending(e => e.ParsedAt)
                        .Select(e => e.Price)
                        .FirstOrDefault(),
                query.Request.SortOrder
            ),
            SortBy.UnifiedPrice => queriable.OrderBy(
                e =>
                    e.PriceHistory.OrderByDescending(e => e.ParsedAt)
                        .Select(e => e.UnifiedPrice)
                        .FirstOrDefault(),
                query.Request.SortOrder
            ),
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

    public static string ToPattern(this string str)
    {
        string[] spec = [@"\", @"%", @"_", @"[", @"]", @"^"];
        foreach (var ch in spec)
            str = str.Replace(ch, @"\" + ch);
        return $"%{str}%";
    }
}


// 1. If product from stage table with exactly same name, shop, amount and unit already exists in database - update price history for existing product (if price has not changed and there are more than 2 entries - update datetime; if price has not changed and there none or one entry - insert new entry; if price has changed - insert new entry)
// 2. If product from stage table with exactly same name exists but either shop or amount or unit differs - create new product entry and price history entry in the existing product group
// 3. If there are no product with given name - add new product entry and price history. Find product group with same normalized name - if it exists then add product to the group, if not - create new group with the normalized name.
// 3.1. Find out best groups for each product without DISTINCT ON for product name
// 3.2. Find out which new groups should be created with distinct names and such that each group has trigram ideally less then 0.9 with all others new groups. Do this in 2 passes - consider all pairs and  use only one from pair if similarity is >0.9 picking name coming first in alphabetical order. Do second same pass among result from previous to mitigate first level of transitivity.
// 3.3. During resolution of the group for products, keep as it is for products that already had a group assigned,  but for products that had score less than or equal 0.9 - again find out best matching group from newly added list (pick the best even if it has score less than 0.9).
// 3.4. Only then update products table with these data
