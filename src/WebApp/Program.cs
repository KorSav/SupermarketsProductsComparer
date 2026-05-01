using ApplicationCore;
using ApplicationCore.Entities.Product;
using ApplicationCore.Services;
using DotNetEnv;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder
    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(4);
        options.LoginPath = "/";
    });

builder.AddInfrastructure();

#if AVOID_PARSING
Console.WriteLine("Started without data parsing");
builder.Services.RemoveAll<RepositoryFreshUpService>();
builder.Services.AddScoped<IShopProductProvider, NoOpProvider>();
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

file class NoOpProvider(ILogger<NoOpProvider> logger) : IShopProductProvider
{
    public IAsyncEnumerable<Product> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Returning no products as parsing result");
        return AsyncEnumerable.Empty<Product>();
    }
}
