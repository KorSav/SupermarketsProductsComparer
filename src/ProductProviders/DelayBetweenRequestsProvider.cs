using PriceComparer.Application.Products.DTOs;
using PriceComparer.Application.Products.Provider;

namespace PriceComparer.ProductProvider;

internal class DelayBetweenRequestsProvider : IProductProvider
{
    readonly PeriodicTimer _timer;
    readonly IProductByNameProvider _providerByName;
    readonly IList<string> _requests;

    internal DelayBetweenRequestsProvider(
        IProductByNameProvider providerByName,
        IList<string> requests,
        TimeSpan period
    )
    {
        _providerByName = providerByName;
        _requests = requests;
        _timer = new(period);
    }

    public async Task<List<ProductInfoDto>> GetProductsAsync(CancellationToken cancellationToken)
    {
        List<ProductInfoDto> result = new(_requests.Count * 30); // random approximate number for products per request
        int i = 0;
        do
        {
            var prods = await _providerByName.GetProductsAsync(_requests[i++], cancellationToken);
            result.AddRange(prods);
            if (i >= _requests.Count)
                break;
        } while (await _timer.WaitForNextTickAsync(cancellationToken));
        return result;
    }
}
