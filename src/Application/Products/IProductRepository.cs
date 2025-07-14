using PriceComparer.Application.Common;
using PriceComparer.Application.Products.DTOs;
using PriceComparer.Application.Products.Types;

namespace PriceComparer.Application.Products;

public interface IProductRepository
{
    Task<IList<StoredProductDto>> GetAllAsync(
        DataWindow dataWindow,
        SortOptions sortOptions,
        CancellationToken cancellationToken
    );
    Task<IList<StoredProductDto>> FindByNameAsync(
        string query,
        DataWindow dataWindow,
        SortOptions sortOptions,
        CancellationToken cancellationToken
    );
    Task UpdateAllStatusAsync(ProductStatus status);
    Task CreateOrUpdateAsync(List<StoredProductDto> products);
    Task DeleteAllWithStatusAsync(ProductStatus status);
}
