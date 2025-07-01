using Microsoft.EntityFrameworkCore;
using PriceComparer.Domain;
using PriceComparer.Domain.Enums;
using PriceComparer.Web.Repository.Data;

namespace PriceComparer.Web.Repository.Impl;

public class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task DeleteAllWithStatusAsync(ProductStatus statusId)
    {
        await _dbContext.Products.Where(p => p.ProductStatus == statusId).ExecuteDeleteAsync();
    }

    public IQueryable<Product> FindByQuery(string query, SortBy sortBy, SortOrder sortOrder)
    {
        return ApplyOrdering(
            _dbContext
                .Products.AsNoTracking()
                .Where(p => EF.Functions.Like(p.Name.ToLower(), $"%{query.ToLower()}%")),
            sortBy,
            sortOrder
        );
    }

    private IQueryable<Product> ApplyOrdering(
        IQueryable<Product> products,
        SortBy sortBy,
        SortOrder sortOrder
    )
    {
        return sortBy switch
        {
            SortBy.Name => sortOrder == SortOrder.Asc
                ? products.OrderBy(p => p.Name)
                : products.OrderByDescending(p => p.Name),
            SortBy.UnifiedPrice => sortOrder == SortOrder.Asc
                ? products.OrderBy(p => p.PriceUnified)
                : products.OrderByDescending(p => p.PriceUnified),
            SortBy.Price => sortOrder == SortOrder.Asc
                ? products.OrderBy(p => p.PriceInitial)
                : products.OrderByDescending(p => p.PriceInitial),
            _ => throw new NotImplementedException(),
        };
    }

    public IQueryable<Product> GetAll(SortBy sortBy, SortOrder sortOrder)
    {
        var products = _dbContext.Products.AsNoTracking();
        return ApplyOrdering(products, sortBy, sortOrder);
    }

    public async Task MapAllProductsAsync(ProductStatus statusId)
    {
        await _dbContext.Products.ExecuteUpdateAsync(prod =>
            prod.SetProperty(e => e.ProductStatus, statusId)
        );
    }

    public async Task SaveAllAsync(List<Product> products)
    {
        foreach (var product in products)
        {
            var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(p =>
                p.FullLinkProduct == product.FullLinkProduct
            );
            if (existingProduct is null)
            {
                await _dbContext.AddAsync(product);
                continue;
            }
            _dbContext.Entry(existingProduct).CurrentValues.SetValues(product);
        }
        await _dbContext.SaveChangesAsync();
    }
}
