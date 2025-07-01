using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PriceComparer.Domain;
using PriceComparer.Domain.Enums;
using PriceComparer.Web.Controllers.Enums;
using PriceComparer.Web.Mappings;
using PriceComparer.Web.Models;
using PriceComparer.Web.Services;
using PriceComparer.Web.Utils;

namespace PriceComparer.Web.Controllers;

public class HomeController : Controller
{
    private readonly ProductService _productService;
    private readonly RequestService _requestService;

    public HomeController(ProductService productService, RequestService requestService)
    {
        _productService = productService;
        _requestService = requestService;
    }

    public async Task<IActionResult> Index(
        string? find,
        int page = 1,
        int pageSize = 24,
        SortBy sortBy = SortBy.Name,
        SortOrder sortOrder = SortOrder.Asc
    )
    {
        bool isOptionChosen = false;
        bool showChooseOption = false;
        PaginatedList<Product> products;
        User? user = Domain.User.FromClaimsPrincipal(User);
        if (user is not null && find is not null)
        {
            Request request = new()
            {
                User = new(), //FIXME: stub
                SortOrder = sortOrder,
                Sort = sortBy,
                Name = find,
            };
            bool isUpdated = await _requestService.UpdateRequestIfExistsAsync(request);
            showChooseOption = true;
            isOptionChosen = isUpdated;
        }
        if (find is null || find.Trim() == string.Empty)
        {
            products = await _productService.GetAllProducts(page, pageSize, sortBy, sortOrder);
        }
        else
        {
            products = await _productService.FindProductsByQueryAsync(
                find,
                page,
                pageSize,
                sortBy,
                sortOrder
            );
        }

        ViewBag.ShowChooseOption = showChooseOption;
        ViewBag.IsOptionChosen = isOptionChosen;
        return View(products.ToProductViewModels());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
