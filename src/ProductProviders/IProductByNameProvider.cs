using PriceComparer.Application.Products.DTOs;

namespace PriceComparer.ProductProvider;

internal interface IProductByNameProvider
{
    Task<List<ProductInfoDto>> GetProductsAsync(
        string prodNameQuery,
        CancellationToken cancellationToken
    );
}
