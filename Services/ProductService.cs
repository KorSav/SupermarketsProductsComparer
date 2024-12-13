using Microsoft.EntityFrameworkCore;
using program.Domain;
using program.Repository;
using program.Utils;

namespace program.Services;

public class ProductService(IProductRepository productRepository)
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<PaginatedList<Product>> GetAllProducts(int pageNo, int pageSize){
        return await PaginatedList<Product>.CreateAsync(_productRepository.GetAll(), pageNo, pageSize);
    }
    public async Task<PaginatedList<Product>> FindProductsByQueryAsync(string query, int pageNo, int pageSize){
        return await PaginatedList<Product>.CreateAsync(_productRepository.FindByQuery(query), pageNo, pageSize);
    }
}