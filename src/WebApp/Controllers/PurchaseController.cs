using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

[Authorize]
[Route("purchases")]
public sealed class PurchasesController : Controller
{
    private readonly IPurchasesService _purchasesService;

    public PurchasesController(IPurchasesService purchasesService)
    {
        _purchasesService = purchasesService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        [FromQuery] PurchasesQuery query,
        CancellationToken cancellationToken
    )
    {
        Guid userId = GetUserId();

        var page = await _purchasesService.FindPageAsync(userId, query, cancellationToken);

        PurchasesViewModel model = new(page, query, page.PagesCount, page.PagesCount);

        return View(model);
    }

    [HttpDelete("{purchaseId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Guid purchaseId, CancellationToken cancellationToken)
    {
        Guid userId = GetUserId();

        await _purchasesService.RemoveAsync(userId, purchaseId, cancellationToken);

        return Ok(new { success = true });
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
