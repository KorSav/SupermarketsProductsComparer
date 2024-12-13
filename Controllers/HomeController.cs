using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using program.Domain;
using program.Domain.Mappings;
using program.Models;
using program.Services;
using program.Utils;

namespace program.Controllers;

public class HomeController : Controller
{
    private readonly ProductService _productService;

    public HomeController(ProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index(string? find, int page = 1, int pageSize = 15)
    {
        PaginatedList<Product> products;
        if (find is null || find.Trim() == string.Empty)
            products = await _productService.GetAllProducts(page, pageSize);
        else
            products = await _productService.FindProductsByQueryAsync(find, page, pageSize);
        var productViewModels = products.ToProductViewModels();
        return View(productViewModels);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
