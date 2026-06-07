using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;
using ApplicationCore.Services;
using ApplicationCore.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Mappings;

namespace WebApp.Controllers;

public class ChosenRequestsController(
    StoredRequestsService storedRequestsService,
    ProductService productService
) : Controller
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var user = User.ToUser();
        var storedRequests = await storedRequestsService.GetAllForUserAsync(user.Id, ct);
        var productLists = new Dictionary<StoredRequest, PaginatedList<Product>>();
        foreach (var stored in storedRequests)
        {
            var productList = await productService.GetProductsAsync(
                stored.Request,
                0,
                4, // productsPerRequest
                ct
            );
            productLists.Add(stored, productList);
        }
        return View(productLists);
    }
}
