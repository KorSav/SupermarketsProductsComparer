namespace PriceComparer.Web.Services.ShopsDataParsing;

public interface IShopDataRetriever
{
    public Task<List<IShopProduct>> GetProductsAsync(string searchQuery);
}