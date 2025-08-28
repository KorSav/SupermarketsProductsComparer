using PriceComparer.Application.Products.DTOs;

namespace PriceComparer.Application.Products.Provider;

public class ParallelProductProvider(IEnumerable<IProductProvider> productProviders)
    : IProductProvider
{
    public async Task<List<ProductInfoDto>> GetProductsAsync(CancellationToken cancellationToken)
    {
        var tasks = productProviders.Select(p => p.GetProductsAsync(cancellationToken));
        var products = await Task.WhenAll(tasks);
        return [.. products.SelectMany(p => p)];
    }
}
