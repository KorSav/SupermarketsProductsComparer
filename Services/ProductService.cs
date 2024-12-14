using Microsoft.EntityFrameworkCore;
using program.Controllers.Enums;
using program.Domain;
using program.Repository;
using program.Utils;

namespace program.Services;

public class ProductService(IProductRepository productRepository)
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<PaginatedList<Product>> GetAllProducts(int pageNo, int pageSize, SortBy sortBy = SortBy.Name, SortOrder sortOrder = SortOrder.Asc)
    {
        var products = _productRepository.GetAll(sortBy, sortOrder);
        return await PaginatedList<Product>.CreateAsync(products, pageNo, pageSize);
    }
    public async Task<PaginatedList<Product>> FindProductsByQueryAsync(string query, int pageNo, int pageSize, SortBy sortBy = SortBy.Name, SortOrder sortOrder = SortOrder.Asc)
    {
        var products = _productRepository.FindByQuery(query, sortBy, sortOrder);
        return await PaginatedList<Product>.CreateAsync(products, pageNo, pageSize);
    }
}