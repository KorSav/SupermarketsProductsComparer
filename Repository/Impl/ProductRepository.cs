using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using program.Controllers.Enums;
using program.Domain;
using program.Domain.Enums;
using program.Repository.Data;

namespace program.Repository.Impl;

public class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task DeleteAllWithStatusAsync(ProductStatusId statusId)
    {
        await _dbContext.Products
            .Where(p => p.ProductStatusId == statusId)
            .ExecuteDeleteAsync();
    }

    public IQueryable<Product> FindByQuery(string query, SortBy sortBy, Controllers.Enums.SortOrder sortOrder)
    {
        return ApplyOrdering(
            _dbContext.Products
                .AsNoTracking()
                .Where(p => EF.Functions.Like(p.Name.ToLower(), $"%{query.ToLower()}%"))
            , sortBy, sortOrder
        );
    }

    private IQueryable<Product> ApplyOrdering(IQueryable<Product> products, SortBy sortBy, Controllers.Enums.SortOrder sortOrder){
        return sortBy switch
        {
            SortBy.Name => sortOrder == Controllers.Enums.SortOrder.Asc
                ? products.OrderBy(p => p.Name)
                : products.OrderByDescending(p => p.Name),
            SortBy.UnifiedPrice => sortOrder == Controllers.Enums.SortOrder.Asc
                ? products.OrderBy(p => p.PriceUnified)
                : products.OrderByDescending(p => p.PriceUnified),
            _ => throw new NotImplementedException()
        };
    }

    public IQueryable<Product> GetAll(SortBy sortBy, Controllers.Enums.SortOrder sortOrder)
    {
        var products = _dbContext.Products.AsNoTracking();
        return ApplyOrdering(products, sortBy, sortOrder);
    }

    public async Task MapAllProductsAsync(ProductStatusId statusId)
    {
        await _dbContext.Products.ExecuteUpdateAsync(prod =>
            prod.SetProperty(e => e.ProductStatusId, statusId));
    }

    public async Task SaveAllAsync(List<Product> products)
    {
        foreach (var product in products)
        {
            var existingProduct = await _dbContext.Products
                .FirstOrDefaultAsync(p =>
                    p.Name == product.Name &&
                    p.ShopId == product.ShopId
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