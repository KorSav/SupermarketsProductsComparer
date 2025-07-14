using PriceComparer.Application.Products.DTOs;

namespace PriceComparer.Application.Products;

public interface IProductProvider
{
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(
        string request,
        CancellationToken cancellationToken
    );
}
