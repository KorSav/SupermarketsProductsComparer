using program.Domain;
using program.Domain.Enums;

namespace program.Repository;

public interface IProductRepository
{
    Task MapAllProductsAsync(ProductStatus statusId);
    Task SaveAllAsync(List<Product> products);
    Task DeleteAllWithStatusAsync(ProductStatus statusId);
    IQueryable<Product> FindByQuery(string query, SortBy sortBy = SortBy.Name, SortOrder sortOrder = SortOrder.Asc);
    IQueryable<Product> GetAll(SortBy sortBy = SortBy.Name, SortOrder sortOrder = SortOrder.Asc);
}