using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparer.Domain;
using PriceComparer.Web.Mappings;
using PriceComparer.Web.Models;
using PriceComparer.Web.Models.Request;
using PriceComparer.Web.Services;
using PriceComparer.Web.Utils;

namespace PriceComparer.Web.Controllers;

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
                request.Name,
                1,
                NoProductsPerFind,
                request.Sort,
                request.SortOrder
            );
            productLists.Add(request.ToRequestViewModel(), productList.ToProductViewModels());
        }
        return View(productLists);
    }
}
