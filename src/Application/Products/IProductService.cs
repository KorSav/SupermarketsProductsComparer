using PriceComparer.Application.Common;
using PriceComparer.Application.Products.DTOs;
using PriceComparer.Application.Products.Provider;
using PriceComparer.Application.Products.Types;

namespace PriceComparer.Application.Products;

public interface IProductService
{
    Task<PaginatedList<ProductInfoDto>> FindProductsByNameAsync(
        string prodName,
        DataPage page,
        SortOptions sortOptions,
        CancellationToken cancellationToken
    );
    Task<PaginatedList<ProductInfoDto>> GetAllProducts(
        DataPage dataPage,
        SortOptions sortOptions,
        CancellationToken cancellationToken
    );
    Task UpdateRepoPeriodicallyUntilCancelAsync(
        IProductProvider provider,
        TimeSpan interval,
        CancellationToken cancellationToken
    );
}
