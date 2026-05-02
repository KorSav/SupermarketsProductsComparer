using ApplicationCore.Entities.Request;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Controllers.Mappings;

namespace WebApp.Controllers.Api;

[ApiController]
[Route("Request")]
public class RequestController(StoredRequestsService requestService) : ControllerBase
{
    private readonly StoredRequestsService _requestService = requestService;

    [HttpPost]
    [Authorize]
    [Route("Toggle")]
    public async Task<IActionResult> Toggle(Request request, CancellationToken ct)
    {
        var user = User.ToUser();
        await _requestService.ToggleAsync(request, user.Id, ct);
        return Ok();
    }
}
