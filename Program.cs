using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using program.Repository;
using program.Repository.Data;
using program.Repository.Impl;
using program.Services;
using program.Services.ShopsDataParsing;
using program.Services.ShopsDataParsing.Fora;
using program.Services.ShopsDataParsing.Fozzy;
using program.Services.ShopsDataParsing.Silpo;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IShopDataRetriever, SilpoDataRetriever>();
builder.Services.AddSingleton<IShopDataRetriever, FozzyDataRetirever>();
builder.Services.AddSingleton<IShopDataRetriever, ForaDataRetriever>();
builder.Services.AddSingleton<ShopProductsGeneralizer>();
builder.Services.AddSingleton<PasswordHasher<string>>();
builder.Services.AddHostedService<ShopsDataParsingService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(4);
        options.LoginPath = "";
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
