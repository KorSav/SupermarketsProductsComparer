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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
