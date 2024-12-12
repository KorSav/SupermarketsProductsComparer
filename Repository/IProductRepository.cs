using program.Domain;
using program.Domain.Enums;

namespace program.Repository;

public interface IProductRepository{
    Task MapAllProductsAsync(ProductStatusId statusId);
    Task SaveAllAsync(List<Product> products);
    Task DeleteAllWithStatusAsync(ProductStatusId statusId);
}