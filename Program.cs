using DotNetEnv;
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
builder.Services.AddSingleton<IShopDataRetriever, SilpoDataRetriever>();
builder.Services.AddSingleton<IShopDataRetriever, FozzyDataRetirever>();
builder.Services.AddSingleton<IShopDataRetriever, ForaDataRetriever>();
builder.Services.AddSingleton<ShopProductsGeneralizer>();
builder.Services.AddHostedService<ShopsDataParsingService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
