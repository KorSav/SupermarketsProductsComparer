using PriceComparer.Application.Common;
using PriceComparer.Application.Products.DTOs;
using PriceComparer.Application.Products.Types;

namespace PriceComparer.Application.Products;

public interface IProductRepository
{
    Task<(IEnumerable<StoredProductDto> products, int totalProductsCount)> GetAllAsync(
        DataWindow dataWindow,
        SortOptions sortOptions,
        CancellationToken cancellationToken
    );
    Task<(IEnumerable<StoredProductDto> products, int totalProductsCount)> FuzzyFindByNameAsync(
        string prodName,
        DataWindow dataWindow,
        SortOptions sortOptions,
        CancellationToken cancellationToken
    );
    Task UpdateAllStatusAsync(ProductStatus status);
    Task CreateOrUpdateAsync(List<StoredProductDto> products);
    Task DeleteAllWithStatusAsync(ProductStatus status);
}
