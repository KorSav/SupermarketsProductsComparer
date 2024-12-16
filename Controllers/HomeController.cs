using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using program.Controllers.Enums;
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

    public async Task<IActionResult> Index(string? find, int page = 1, int pageSize = 24, SortBy sortBy = SortBy.Name, SortOrder sortOrder = SortOrder.Asc)
    {
        PaginatedList<Product> products;
        User? user = Domain.User.FromClaimsPrincipal(User);
        if (user is not null && find is not null)
            products = await _productService.FindProductsByQueryAsync(user, find, page, pageSize, sortBy, sortOrder);
        else if (find is null || find.Trim() == string.Empty)
            products = await _productService.GetAllProducts(page, pageSize, sortBy, sortOrder);
        else
            products = await _productService.FindProductsByQueryAsync(find, page, pageSize, sortBy, sortOrder);
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
