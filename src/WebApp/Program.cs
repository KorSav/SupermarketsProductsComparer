using ApplicationCore;
using ApplicationCore.Services;
using DotNetEnv;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder
    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(4);
        options.LoginPath = "/";
    });

builder.AddApplicationCore();
builder.AddInfrastructure();

#if AVOID_PARSING
Console.WriteLine("Started without data parsing");
var hosted = builder.Services.FirstOrDefault(s =>
    s.ImplementationType == typeof(RepositoryFreshUpService)
);
builder.Services.Remove(hosted!);
builder.Services.RemoveAll<IShopProductProvider>();
// builder.Services.AddScoped<IShopProductProvider, NoOpProvider>();
#endif

var app = builder.Build();
await app.Services.ThrowIfDbIsNotAccessibleAsync();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}");

app.Run();

// file class NoOpProvider(ILogger<NoOpProvider> logger) : IShopProductProvider
// {
//     public IAsyncEnumerable<Product> GetAllAsync(CancellationToken cancellationToken)
//     {
//         logger.LogInformation("Returning no products as parsing result");
//         return AsyncEnumerable.Empty<Product>();
//     }
// }
