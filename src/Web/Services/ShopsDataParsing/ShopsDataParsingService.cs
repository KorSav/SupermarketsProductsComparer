using PriceComparer.Domain.Enums;
using PriceComparer.Web.Mappings;
using PriceComparer.Web.Repository;

namespace PriceComparer.Web.Services.ShopsDataParsing;

public class ShopsDataParsingService(
    ShopProductsGeneralizer shopProductsGeneralizer,
    IEnumerable<IShopDataRetriever> shopDataRetrievers,
    IServiceScopeFactory serviceScopeFactory,
    IConfiguration configuration
) : BackgroundService
{
    private readonly ShopProductsGeneralizer _shopProductsGeneralizer = shopProductsGeneralizer;
    private readonly IEnumerable<IShopDataRetriever> _shopDataRetrievers = shopDataRetrievers;
    private readonly TimeSpan _interval = TimeSpan.FromHours(
        configuration.GetRequiredSection("Delays:DbFreshUpHrs").Get<double>()
    );
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    private readonly TimeSpan _delayRequests = TimeSpan.FromSeconds(
        configuration.GetRequiredSection("Delays:RequestsSecs").Get<double>()
    );

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var parsingStartTime = DateTime.UtcNow;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var requestRepository =
                    scope.ServiceProvider.GetRequiredService<IRequestRepository>();
                var productRepository =
                    scope.ServiceProvider.GetRequiredService<IProductRepository>();
                await productRepository.MapAllProductsAsync(ProductStatus.NeedRemoval);
                var requests = await requestRepository.GetAllRequestsOfUserAsync(-1);
                var lastRequest = requests.Last();
                foreach (var request in requests)
                {
                    Console.WriteLine($"Quering '{request.Name}' among all shops");
                    var retrievalTasks = new List<Task<List<IShopProduct>>>();
                    foreach (var shopDataRetriever in _shopDataRetrievers)
                    {
                        retrievalTasks.Add(shopDataRetriever.GetProductsAsync(request.Name));
                    }
                    var productsList = await Task.WhenAll(retrievalTasks);
                    var products = productsList.SelectMany(ps => ps).ToList();
                    List<GeneralProduct> generalProducts = _shopProductsGeneralizer.Generalize(
                        products
                    );
                    await productRepository.SaveAllAsync(generalProducts.ToProducts());
                    if (request != lastRequest)
                        await Task.Delay(_delayRequests, stoppingToken);
                }
                await productRepository.DeleteAllWithStatusAsync(ProductStatus.NeedRemoval);
            }
            var parsingDuration = DateTime.UtcNow - parsingStartTime;
            var remainingTime = _interval - parsingDuration;
            if (remainingTime > TimeSpan.Zero)
            {
                await Task.Delay(remainingTime, stoppingToken);
            }
        }
    }
}
