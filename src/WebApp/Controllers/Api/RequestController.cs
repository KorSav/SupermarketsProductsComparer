using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using program.DataSources.Repository.Entities;
using program.Domain.Entities;
using program.Domain.Entities.Request;
using program.Domain.Mappings;
using program.Domain.Services;
using program.Models.Request;

namespace program.Api;

[ApiController]
[Route("Request")]
public class RequestController(StoredRequestsService requestService) : Controller
{
    private readonly StoredRequestsService _requestService = requestService;

    [HttpPost]
    [Authorize]
    [Route("Toggle")]
    public async Task<IActionResult> Toggle(RequestViewModel requestViewModel)
    {
        User user = Domain.Entities.User.FromClaimsPrincipal(User)!;
        Request request = requestViewModel.ToRequest(user.Id);
        await _requestService.ToggleRequestAsync(request);
        return Ok();
    }
}