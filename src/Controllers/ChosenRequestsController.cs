using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using program.Domain;
using program.Domain.Mappings;
using program.Models;
using program.Models.Request;
using program.Services;
using program.Utils;

namespace program.Controllers;

public class ChosenRequestsController : Controller
{
    private readonly RequestService _requestService;
    private readonly ProductService _productService;
    public ChosenRequestsController(RequestService requestService, ProductService productService)
    {
        _requestService = requestService;
        _productService = productService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index(int NoProductsPerFind = 4)
    {
        User user = Domain.User.FromClaimsPrincipal(User)!;
        var requests = await _requestService.GetRequestsAsync(user);
        var productLists = new Dictionary<RequestInfoViewModel, PaginatedList<ProductViewModel>>();
        foreach (var request in requests)
        {
            var productList = await _productService.FindProductsByQueryAsync(
                request.Name, 1, NoProductsPerFind,
                request.SortId, request.SortOrderId
            );
            productLists.Add(
                request.ToRequestViewModel(),
                productList.ToProductViewModels()
            );
        }
        return View(productLists);
    }
}