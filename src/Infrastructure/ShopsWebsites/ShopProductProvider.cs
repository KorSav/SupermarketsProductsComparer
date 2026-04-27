using System.Diagnostics;
using System.Runtime.CompilerServices;
using ApplicationCore;
using ApplicationCore.Entities.Product;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.ShopsWebsites;

public class ShopProductProvider(
    IEnumerable<IShopDataRetriever> shopDataRetrievers,
    IConfiguration config,
    ILogger<ShopProductProvider> logger,
    IOptionsMonitor<ShopProductProviderOptions> delaySettings
) : IShopProductProvider
{
    private readonly IReadOnlyList<IShopDataRetriever> _retrievers = shopDataRetrievers
        .ToList()
        .AsReadOnly();
    private readonly IConfiguration _config = config;
    private readonly ILogger _logger = logger;
    private readonly IOptionsMonitor<ShopProductProviderOptions> _delaySettings = delaySettings;

    /// <summary>
    /// FIXME: retrieval is synchronized with kind of barrier waiting for all retriever to finish before new retrieval starts
    /// Also it heavily depends on callers retrieval speed
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<Product> GetAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        Stopwatch sw = new();
        for (int i = 0; i < _requests.Count; i++)
        {
            if (i > 0)
            {
                var delay = _delaySettings.CurrentValue.DelayBetweenRequests;
                var passed = sw.Elapsed;
                if (passed < delay)
                    await Task.Delay(delay - passed, cancellationToken);
            }
            var request = _requests[i];
            _logger.LogInformation("Start quering '{Request}' among shops", request);
            var tasksResult = await Task.WhenAll(
                _retrievers.Select(r => r.GetProductsAsync(request, cancellationToken))
            ); // problem that may lead to unnecessary connections close/create

            sw.Restart();
            var products = tasksResult.SelectMany(_ => _).Select(sp => sp.ToProduct(_config));
            foreach (var p in products)
                yield return p;
        }
    }

    private static readonly List<string> _requests =
    [
        "м'ясо",
        "сосиски",
        "сардельки",
        "овоч",
        "хліб",
        "фрукт",
        "молоко",
        "сир",
        "йогурт",
        "яйця",
        "риба",
        "крупа",
        "макаронні вироби",
        "цукор",
        "сіль",
        "олія",
        "оцет",
        "кава зерно",
        "кава розчинна",
        "чай",
        "шоколад",
        "печиво",
        "торт",
        "морозиво",
        "сік",
        "вода",
        "напій",
        "ковбаса",
        "консерви",
        "соус",
        "спеції",
        "шампунь",
        "зубна паста",
        "мило",
        "туалетний папір",
        "пральний порошок",
        "корм",
        "підгузки",
    ];
}
