namespace Infrastructure.ShopsWebsites;

public interface IShopDataRetriever
{
    public Task<List<IShopProduct>> GetProductsAsync(
        string searchQuery,
        CancellationToken cancellationToken
    );
}
