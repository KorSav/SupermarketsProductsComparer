using program.Controllers.Enums;
using program.Domain;
using program.Domain.Enums;

namespace program.Repository;

public interface IProductRepository
{
    Task MapAllProductsAsync(ProductStatusId statusId);
    Task SaveAllAsync(List<Product> products);
    Task DeleteAllWithStatusAsync(ProductStatusId statusId);
    IQueryable<Product> FindByQuery(string query, SortBy sortBy = SortBy.Name, SortOrderId sortOrder = SortOrderId.Asc);
    IQueryable<Product> GetAll(SortBy sortBy = SortBy.Name, SortOrderId sortOrder = SortOrderId.Asc);
}