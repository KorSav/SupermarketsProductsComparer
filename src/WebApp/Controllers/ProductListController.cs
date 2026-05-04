using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

[Authorize]
[Route("product-list")]
public sealed class ProductListController(IProductListService productListService) : Controller
{
    private readonly IProductListService _productListService = productListService;

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        Guid userId = GetUserId();

        ProductListViewModel model = await _productListService.GetCurrentAsync(
            userId,
            cancellationToken
        );

        return View(model);
    }

    [HttpPatch("entries/{entryId:guid}/amount")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAmount(
        Guid entryId,
        [FromBody] UpdateProductListEntryAmountRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request.Amount <= 0)
        {
            return BadRequest(
                new { success = false, message = "Amount must be greater than zero." }
            );
        }

        Guid userId = GetUserId();

        ProductListViewModel updatedModel = await _productListService.UpdateEntryAmountAsync(
            userId,
            entryId,
            request.Amount,
            cancellationToken
        );

        ProductListEntryViewModel? updatedEntry = updatedModel.Entries.FirstOrDefault(x =>
            x.EntryId == entryId
        );

        return Ok(
            new
            {
                success = true,
                entryTotal = updatedEntry?.TotalPrice ?? 0,
                listTotal = updatedModel.TotalPrice,
                isEmpty = updatedModel.IsEmpty,
            }
        );
    }

    [HttpDelete("entries/{entryId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveEntry(Guid entryId, CancellationToken cancellationToken)
    {
        Guid userId = GetUserId();

        ProductListViewModel updatedModel = await _productListService.RemoveEntryAsync(
            userId,
            entryId,
            cancellationToken
        );

        return Ok(
            new
            {
                success = true,
                listTotal = updatedModel.TotalPrice,
                isEmpty = updatedModel.IsEmpty,
            }
        );
    }

    [HttpPost("store")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StoreCurrentList(CancellationToken cancellationToken)
    {
        Guid userId = GetUserId();

        Guid purchaseId = await _productListService.StoreCurrentAsPurchaseAsync(
            userId,
            cancellationToken
        );

        return Ok(
            new
            {
                success = true,
                purchaseId,
                redirectUrl = Url.Action("Index", "Purchases"),
            }
        );
    }

    private Guid GetUserId()
    {
        string? rawUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? User.FindFirstValue("userId");

        if (!Guid.TryParse(rawUserId, out Guid userId))
            throw new InvalidOperationException("Authenticated user id is missing or invalid.");

        return userId;
    }
}
