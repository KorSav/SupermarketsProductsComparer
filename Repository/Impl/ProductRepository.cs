using Microsoft.EntityFrameworkCore;
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

    public IQueryable<Product> FindByQuery(string query)
    {
        return GetAll()
            .Where(p => EF.Functions.Like(p.Name.ToLower(), $"%{query.ToLower()}%"));
    }

    public IQueryable<Product> GetAll()
    {
        return _dbContext.Products
            .AsNoTracking();
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