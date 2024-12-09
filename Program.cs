using program.Services.ShopsDataParsing;
using program.Services.ShopsDataParsing.Fozzy;
using program.Services.ShopsDataParsing.Silpo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IShopDataRetriever, SilpoDataRetriever>();
builder.Services.AddSingleton<IShopDataRetriever, FozzyDataRetirever>();
builder.Services.AddSingleton<ShopProductsGeneralizer>();
builder.Services.AddHostedService<ShopsDataParsingService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
