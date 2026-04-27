using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using program.DataSources.Repository.Entities;
using program.Domain.Entities;
using program.Domain.Entities.Product;
using program.Domain.Entities.Request;
using program.Domain.Mappings;
using program.Domain.Services;
using program.Domain.Utils;
using program.Models;

namespace program.Controllers;

public class HomeController : Controller
{
    private readonly ProductService _productService;
    private readonly StoredRequestsService _requestService;

    public HomeController(ProductService productService, StoredRequestsService requestService)
    {
        _productService = productService;
        _requestService = requestService;
    }

    public async Task<IActionResult> Index(string? find, int page = 1, int pageSize = 24, SortBy sortBy = SortBy.Name, SortOrder sortOrder = SortOrder.Asc)
    {
        bool isOptionChosen = false;
        bool showChooseOption = false;
        PaginatedList<Product> products;
        User? user = Domain.Entities.User.FromClaimsPrincipal(User);
        if (user is not null && find is not null){
            Request request = new(){
                UserId = user.Id,
                SortOrderId = sortOrder,
                SortId = sortBy,
                Name = find
            };
            bool isUpdated = await _requestService.UpdateRequestIfExistsAsync(request);
            showChooseOption = true;
            isOptionChosen = isUpdated;
        }
        if (find is null || find.Trim() == string.Empty){
            products = await _productService.GetAllProducts(page, pageSize, sortBy, sortOrder);
        }
        else{
            products = await _productService.FindProductsByQueryAsync(find, page, pageSize, sortBy, sortOrder);
        }

        ViewBag.ShowChooseOption = showChooseOption;
        ViewBag.IsOptionChosen = isOptionChosen;
        return View(products.ToProductViewModels());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
