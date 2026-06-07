using ApplicationCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.DTOs;
using WebApp.Mappings;

namespace WebApp.Api;

[ApiController]
[Route("Request")]
public class RequestController(StoredRequestsService requestService) : ControllerBase
{
    private readonly StoredRequestsService _requestService = requestService;

    [HttpPost]
    [Authorize]
    [Route("Toggle")]
    public async Task<IActionResult> Toggle(ToggleRequestDto toggleRequest, CancellationToken ct)
    {
        var user = User.ToUser();
        await _requestService.ToggleAsync(toggleRequest.ToRequest(), user.Id, ct);
        return Ok();
    }
}
