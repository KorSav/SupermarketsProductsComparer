using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using program.Domain;
using program.Domain.Mappings;
using program.Models;
using program.Models.User;
using program.Services;

namespace program.Api;

public class AuthController(UserService userService) : Controller
{
    private readonly UserService _userService = userService;

    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody] UserRegisterViewModel userViewModel)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { Errors = errors });
        }

        User user = userViewModel.ToUser();
        if (await _userService.TryRegister(user) is false)
            return BadRequest(new { Errors = new List<string> { "Користувач з даним іменем чи поштою вже існує" } });

        await SignInHttpCtxAsync(user);
        return Ok();
    }


    [HttpPost]
    public async Task<IActionResult> LogIn([FromBody] UserLogInViewModel userViewModel)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { Errors = errors });
        }

        User user = userViewModel.ToUser();
        if (await _userService.IsRegisteredAsync(user) is false)
            return BadRequest("Користувач не зареєстрований");
        await SignInHttpCtxAsync(user);
        return Ok();
    }

    [Authorize]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    private async Task SignInHttpCtxAsync(User user)
    {
        var claims = new List<Claim>() {
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity)
        );
    }
}