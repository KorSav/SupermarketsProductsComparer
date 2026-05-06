using System.Diagnostics;
using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;
using ApplicationCore.Services;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Controllers.DTOs;
using WebApp.Controllers.Mappings;
using WebApp.Models;
using static WebApp.Models.ProductViewModel;

namespace WebApp.Controllers;

public class ProductsController(ProductService productService, AppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(
        [FromQuery(Name = "")] FindRequestDto dto,
        CancellationToken ct
    )
    {
        var request = dto.ToRequest();
        if (User.Identity is null or { IsAuthenticated: false })
        {
            var result = await productService.GetProductsAsync(
                request,
                dto.Page,
                dto.PageLimit,
                ct
            );
            return View(new HomeViewModel(result, request, isOptionChosen: null));
        }

        var user = User.ToUser();
        var authnGetResult = await productService.AuthnGetProductsAsync(
            request,
            dto.Page,
            dto.PageLimit,
            user.Id,
            ct
        );
        bool? isOptionChosen = request.SearchString is "" ? null : authnGetResult.IsStored;
        return View(new HomeViewModel(authnGetResult.Products, request, isOptionChosen));
    }

    [HttpGet("[controller]/{id:guid}")]
    public async Task<IActionResult> ProductDetails(Guid id, CancellationToken ct)
    {
        var requestedProduct = await dbContext
            .Products.AsNoTracking()
            .Include(p => p.PriceHistory)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (requestedProduct is null)
            return NotFound();

        var efProductsFromSameGroup = await dbContext
            .Products.AsNoTracking()
            .Include(p => p.PriceHistory)
            .Where(p => p.ProductGroupId == requestedProduct.ProductGroupId)
            .ToListAsync(ct);

        var productDetails = new List<ProductDetail>();
        var priceHistories = new List<PriceHistory>();

        foreach (
            var efProduct in efProductsFromSameGroup
                .OrderByDescending(p => p.Id == id)
                .ThenBy(p => p.Shop)
                .ThenBy(p => p.Amount)
                .ThenBy(p => p.Unit)
        )
        {
            var product = efProduct.ToCoreProduct();
            var unifiedProduct = product.WithUnifiedPrice();

            var mockPriceHistory = GenerateMockPriceHistory(
                product.Id,
                product.Price,
                product.Measure
            );

            var averagePrice = mockPriceHistory.History.Average(e => e.Price);

            productDetails.Add(
                new ProductDetail(
                    efProduct.Id,
                    product.DisplayName,
                    efProduct.Shop,
                    efProduct.FullLinkImage,
                    efProduct.FullLinkProduct,
                    product.Price,
                    UnifiedPrice: unifiedProduct.Price,
                    AveragePrice: averagePrice,
                    UnifiedMeasure: unifiedProduct.Measure,
                    Measure: new Measure(efProduct.Amount, efProduct.Unit)
                )
            );

            priceHistories.Add(mockPriceHistory);
        }

        var requestedCoreProduct = requestedProduct.ToCoreProduct();

        var recommended = await productService.GetProductsAsync(
            new(requestedCoreProduct.Name, SortBy.Name, SortOrder.Asc),
            0,
            8,
            ct
        );

        var filterGroupName = requestedCoreProduct.NormalizedName;
        ProductViewModel model = new(
            productDetails,
            priceHistories,
            recommended
                .Where(p => p.NormalizedName != filterGroupName)
                .Select(p => new HomeViewModel.ProductInfo(p))
                .ToList()
        );

        return View(model);
    }

    private static PriceHistory GenerateMockPriceHistory(
        Guid productId,
        decimal currentPrice,
        Measure measure
    )
    {
        const int days = 5;
        const decimal maxDeviation = 5m;

        List<PriceEntry> history = [];

        decimal scale = measure.ScaleFactorTo(Product.UnifiedMeasure(measure));
        for (int i = days - 1; i >= 0; i--)
        {
            decimal deviation = Math.Round(
                (decimal)(
                    Random.Shared.NextDouble() * (double)(maxDeviation * 2) - (double)maxDeviation
                ),
                2
            );
            decimal price = Math.Max(0.01m, currentPrice + deviation);

            history.Add(
                new PriceEntry(
                    Price: price,
                    UnifiedPrice: price * scale,
                    Date: DateTime.UtcNow.Date.AddDays(-i)
                )
            );
        }

        return new PriceHistory(productId, history);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
