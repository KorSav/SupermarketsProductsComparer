namespace program.Services.ShopsDataParsing;

public interface IShopDataRetriever
{
    public Task<List<IShopProduct>> GetProductsAsync(string searchQuery);
}