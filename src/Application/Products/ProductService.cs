using PriceComparer.Application.Common;
using PriceComparer.Application.Products.DTOs;
using PriceComparer.Application.Products.Types;

namespace PriceComparer.Application.Products;

public class ProductService(IProductRepository productRepo)
{
    readonly IProductRepository _repo = productRepo;

    /// <summary>
    /// Adds data into product repo infinitely in a loop with given time interval until cancelled
    /// </summary>
    public async Task UpdateRepoFromProviderAsync(
        IProductProvider provider,
        TimeSpan interval,
        CancellationToken cancellationToken
    )
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var tStart = DateTime.UtcNow;
            await _repo.UpdateAllStatusAsync(ProductStatus.NeedRemoval);
            // move requests into product provider
            foreach (var request in _requests)
            {
                Console.WriteLine($"Quering '{request}'");
                var providedDtos = await provider.GetProductsAsync(request, cancellationToken);
                var unifiedProducts = providedDtos
                    .Select(dto => new StoredProductDto(dto, dto.ProductPrice.AsUnified()))
                    .ToList();
                await _repo.CreateOrUpdateAsync(unifiedProducts);
                if (cancellationToken.IsCancellationRequested)
                {
                    await _repo.UpdateAllStatusAsync(ProductStatus.Updated);
                    return;
                }
            }
            await _repo.DeleteAllWithStatusAsync(ProductStatus.NeedRemoval);
            var duration = DateTime.UtcNow - tStart;
            var waitForNextRetrieval = interval - duration;
            if (waitForNextRetrieval > TimeSpan.Zero)
                await Task.Delay(waitForNextRetrieval, cancellationToken);
        }
    }

    public async Task<IReadOnlyList<StoredProductDto>> GetAllAsync(
        DataPage dataPage,
        SortOptions sortOptions,
        CancellationToken cancellationToken
    ) =>
        (await _repo.GetAllAsync(dataPage.ToWindow(), sortOptions, cancellationToken)).AsReadOnly();

    public async Task<IReadOnlyList<StoredProductDto>> FindByQueryAsync(
        DataPage dataPage,
        RequestDto request,
        CancellationToken cancellationToken
    ) =>
        (
            await _repo.FindByNameAsync(
                request.Query,
                dataPage.ToWindow(),
                request.SortOptions,
                cancellationToken
            )
        ).AsReadOnly();

    static readonly string[] _requests =
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
