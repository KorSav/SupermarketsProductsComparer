using PriceComparer.Application.Products.DTOs;

namespace PriceComparer.Application.Products.Provider;

public interface IProductProvider
{
    Task<List<ProductInfoDto>> GetProductsAsync(CancellationToken cancellationToken);
}
