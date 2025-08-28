using Microsoft.Extensions.Logging;
using PriceComparer.Application.Products.Types;

namespace PriceComparer.Application.Products.Provider;

public interface IProductProviderFactory
{
    IProductProvider Create(Shop shop, HttpClient client, ILoggerFactory loggerFactory);
    IEnumerable<IProductProvider> CreateAll(HttpClient client, ILoggerFactory loggerFactory);
}
