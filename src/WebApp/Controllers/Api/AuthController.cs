using ApplicationCore.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Controllers.DTOs;
using WebApp.Controllers.Mappings;

namespace WebApp.Controllers.Api;

public class AuthController(UserService userService) : ControllerBase
{
    const string _scheme = CookieAuthenticationDefaults.AuthenticationScheme;

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userViewModel)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new { Errors = errors });
        }

        var result = await userService.TryRegisterAsync(
            userViewModel.Name,
            userViewModel.Email,
            userViewModel.Password
        );
        if (!result.IsSuccess)
            return BadRequest(
                new { Errors = result.ErrorList.Select(e => e.Description).Distinct() }
            );

        await HttpContext.SignInAsync(result.Value.ToClaimsPrincipal(_scheme));
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userViewModel)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new { Errors = errors });
        }

        var result = await userService.TryLoginAsync(
            userViewModel.Name,
            userViewModel.Email,
            userViewModel.Password
        );
        if (!result.IsSuccess)
            return BadRequest(
                new { Errors = result.ErrorList.Select(e => e.Description).Distinct() }
            );
        await HttpContext.SignInAsync(result.Value.ToClaimsPrincipal(_scheme));
        return Ok();
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}
