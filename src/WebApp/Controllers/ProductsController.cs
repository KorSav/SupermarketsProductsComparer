using System.Diagnostics;
using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Controllers.DTOs;
using WebApp.Controllers.Mappings;
using WebApp.Models;
using static WebApp.Models.ProductViewModel;

namespace WebApp.Controllers;

public class ProductsController(ProductService productService) : Controller
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

    [HttpGet("[controller]/{id:int}")]
    public async Task<IActionResult> ProductDetails(int id, CancellationToken ct)
    {
        ProductViewModel model = /*id.HasValue
            ? await productService.GetProductViewModel(id.Value, ct)
            :*/
        await GetMockProductViewModelAsync(ct);

        return View(model);
    }

    private async Task<ProductViewModel> GetMockProductViewModelAsync(CancellationToken ct)
    {
        List<ProductDetail> details =
        [
            new(
                0,
                "Pepsi Cola fora",
                Shop.Fora,
                "https://content.fora.ua/sku/ecommerce/99/480x480wwm/991339_480x480wwm_a79f00e3-e798-33b3-1e0e-3bee95e8509b.png",
                "https://fora.ua/product/napii-pepsi-pepsi-kola-bezalkogolnyi-sylnogazovanyi-991339",
                31.9m,
                29,
                30,
                new Measure(1, MeasureUnit.Litre),
                new Measure(1.25m, MeasureUnit.Litre)
            ),
            new(
                2,
                "Pepsi Cola silpo",
                Shop.Silpo,
                "https://images.silpo.ua/v2/products/300x300/webp/7444267e-da77-4cf7-9d2e-d0b621f1a703.png",
                "https://silpo.ua/product/napii-pepsi-pepsi-nul-tsukru-bezalkogolnyi-sylnogazovanyi-991340",
                52.49m,
                43.5m,
                40m,
                new Measure(1, MeasureUnit.Litre),
                new Measure(1.25m, MeasureUnit.Litre)
            ),
            new(
                1,
                "Pepsi Cola silpo 2",
                Shop.Silpo,
                "https://images.silpo.ua/v2/products/300x300/webp/7444267e-da77-4cf7-9d2e-d0b621f1a703.png",
                "https://silpo.ua/product/napii-pepsi-pepsi-nul-tsukru-bezalkogolnyi-sylnogazovanyi-991340",
                33m,
                30m,
                41m,
                new Measure(1, MeasureUnit.Litre),
                new Measure(2, MeasureUnit.Litre)
            ),
        ];

        List<PriceHistory> chart =
        [
            new(
                0,
                [
                    new(30m, 28m, DateTime.UtcNow.AddDays(-5)),
                    new(32m, 29.5m, DateTime.UtcNow.AddDays(-4)),
                    new(29m, 27.3m, DateTime.UtcNow.AddDays(-3)),
                    new(31.9m, 29m, DateTime.UtcNow.AddDays(-2)),
                ]
            ),
            new(
                1,
                [
                    new(35m, 32.7m, DateTime.UtcNow.AddDays(-5)),
                    new(32m, 29.5m, DateTime.UtcNow.AddDays(-4)),
                    new(33.7m, 31.2m, DateTime.UtcNow.AddDays(-3)),
                    new(33m, 30m, DateTime.UtcNow.AddDays(-2)),
                ]
            ),
            new(
                2,
                [
                    new(40m, 36.3m, DateTime.UtcNow.AddDays(-5)),
                    new(42m, 40m, DateTime.UtcNow.AddDays(-4)),
                    new(46m, 43.4m, DateTime.UtcNow.AddDays(-3)),
                    new(48m, 45.7m, DateTime.UtcNow.AddDays(-2)),
                ]
            ),
        ];

        var recommended = await productService.GetProductsAsync(
            new("пепсі", SortBy.Name, SortOrder.Asc),
            0,
            8,
            ct
        );

        return new ProductViewModel(
            details,
            chart,
            recommended.Select(p => new HomeViewModel.ProductInfo(p)).ToList()
        );
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
