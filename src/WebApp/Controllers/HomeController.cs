using System.Diagnostics;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Controllers.DTOs;
using WebApp.Controllers.Mappings;
using WebApp.Models;

namespace WebApp.Controllers;

public class HomeController(ProductService productService) : Controller
{
    public async Task<IActionResult> Index(
        [FromQuery(Name = "")] RequestDto dto,
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
        return View(new HomeViewModel(authnGetResult, request));
        // // bool isOptionChosen = false;
        // // bool showChooseOption = false;
        // // // PaginatedList<Product> products;
        // // User? user = Domain.Entities.User.FromClaimsPrincipal(User);
        // // if (user is not null && searchString is not null)
        // // {
        // //     Request request = new()
        // //     {
        // //         UserId = user.Id,
        // //         SortOrderId = sortOrder,
        // //         SortId = sortBy,
        // //         Name = searchString,
        // //     };
        // //     bool isUpdated = await _requestService.UpdateRequestIfExistsAsync(request);
        // //     showChooseOption = true;
        // //     isOptionChosen = isUpdated;
        // // }
        // // if (searchString is null || searchString.Trim() == string.Empty)
        // // {
        // //     products = await _productService.GetAllProducts(page, pageSize, sortBy, sortOrder);
        // // }
        // // else
        // // {
        // //     products = await _productService.FindProductsByQueryAsync(
        // //         searchString,
        // //         page,
        // //         pageSize,
        // //         sortBy,
        // //         sortOrder
        // //     );
        // // }

        // // ViewBag.ShowChooseOption = showChooseOption;
        // // ViewBag.IsOptionChosen = isOptionChosen;
        // return View(products.ToProductViewModels());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
