using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using program.Controllers.Enums;
using program.Domain;
using program.Repository;
using program.Utils;

namespace program.Services;

public class ProductService(
    IProductRepository productRepository,
    IRequestRepository requestRepository,
    IUserRepository userRepository
)
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IRequestRepository _requestRepository = requestRepository;
    private readonly IUserRepository _userRepository = userRepository;

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

    public async Task<PaginatedList<Product>> FindProductsByQueryAsync(User user, string query, int pageNo, int pageSize, SortBy sortBy = SortBy.Name, SortOrder sortOrder = SortOrder.Asc)
    {
        var productList = await FindProductsByQueryAsync(query, pageNo, pageSize, sortBy, sortOrder);
        if (productList.TotalPages == 0)
            return productList;

        User foundUser = await _userRepository.GetUserByNameAsync(user.Name) ??
            throw new AuthenticationException($"User '{user.Name}' is not present in db");
        Request request = new() {
            UserId = foundUser.Id,
            Name = query
        };
        await _requestRepository.SaveRequestAsync(request);
        return productList;
    }
}