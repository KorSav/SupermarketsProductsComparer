using ApplicationCore;
using ApplicationCore.Services;
using DotNetEnv;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApp.Services;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IProductListService, EfProductListService>();
builder.Services.AddScoped<IPurchasesService, EfPurchasesService>();

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

app.MapControllerRoute(name: "default", pattern: "{controller=Products}/{action=Index}");

await app.RunAsync();

// file class NoOpProvider(ILogger<NoOpProvider> logger) : IShopProductProvider
// {
//     public IAsyncEnumerable<Product> GetAllAsync(CancellationToken cancellationToken)
//     {
//         logger.LogInformation("Returning no products as parsing result");
//         return AsyncEnumerable.Empty<Product>();
//     }
// }

// var httpClient = new HttpClient() { BaseAddress = new Uri("http://fozzyshop.ua/") };

// httpClient.DefaultRequestHeaders.Add(
//     "User-Agent",
//     "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36"
// );
// await using var respStream = await httpClient.GetStreamAsync("/sitemap/sitemap.xml");

// await using var fs = File.Create("result.html");
// await respStream.CopyToAsync(fs);
