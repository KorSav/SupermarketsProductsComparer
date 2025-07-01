using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparer.Domain;
using PriceComparer.Web.Mappings;
using PriceComparer.Web.Models.Request;
using PriceComparer.Web.Services;

namespace PriceComparer.Api;

[ApiController]
[Route("Request")]
public class RequestController(RequestService requestService) : Controller
{
    private readonly RequestService _requestService = requestService;

    [HttpPost]
    [Authorize]
    [Route("Toggle")]
    public async Task<IActionResult> Toggle(RequestViewModel requestViewModel)
    {
        User user = Domain.User.FromClaimsPrincipal(User)!;
        Request request = requestViewModel.ToRequest(user.Id);
        await _requestService.ToggleRequestAsync(request);
        return Ok();
    }
}
