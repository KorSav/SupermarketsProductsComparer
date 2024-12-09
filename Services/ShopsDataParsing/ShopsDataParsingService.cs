namespace program.Services.ShopsDataParsing;

public class ShopsDataParsingService(ShopProductsGeneralizer shopProductsGeneralizer, IEnumerable<IShopDataRetriever> shopDataRetrievers) : BackgroundService
{
    private readonly ShopProductsGeneralizer _shopProductsGeneralizer = shopProductsGeneralizer;
    private readonly IEnumerable<IShopDataRetriever> _shopDataRetrievers = shopDataRetrievers;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var shopDataRetriever in _shopDataRetrievers)
            {
                List<IShopProduct> products = await shopDataRetriever.GetProductsAsync("яблуко");
                List<GeneralProduct> generalProducts = _shopProductsGeneralizer.Generalize(products);
                foreach (var product in generalProducts)
                {
                    Console.WriteLine($"{product.Name}: {product.Price}UAH per {product.Measure} from {product.Shop}, product {product.FullLinkProduct}, image {product.FullLinkImage}");
                }
            }
            await Task.Delay(_interval, stoppingToken);
        }
    }

}