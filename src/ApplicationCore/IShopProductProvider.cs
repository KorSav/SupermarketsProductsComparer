using ApplicationCore.Entities.Product;

namespace ApplicationCore;

public interface IShopProductProvider
{
    IAsyncEnumerable<Product> GetAllAsync(CancellationToken cancellationToken);
}
