using ApplicationCore;
using ApplicationCore.Entities.Product;
using Infrastructure.Repository;
using Infrastructure.ShopsWebsites;
using Infrastructure.ShopsWebsites.Fora;
using Infrastructure.ShopsWebsites.Fozzy;
using Infrastructure.ShopsWebsites.Silpo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class ApplicationBuilderExtension
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder) =>
        builder.AddRepositories().AddShopsWebsitesDataRetrieval();

    private static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
    {
        // builder.Services.AddSingleton<PasswordHasher<string>>();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options
                .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        );
        builder
            .Services.AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IBulkUpsertScope, BulkUpsertScope>()
            .AddScoped<IRequestRepository, RequestRepository>()
            .AddScoped<IUserRepository, UserRepository>();
        return builder;
    }

    private static IHostApplicationBuilder AddShopsWebsitesDataRetrieval(
        this IHostApplicationBuilder builder
    )
    {
        var config = builder.Configuration;
        foreach (var shop in Enum.GetValues<Shop>())
        {
            if (shop is Shop.Fozzy)
            {
                builder.Services.AddDataRetrieverOptions<FozzyDataRetrieverOptions>(
                    config,
                    shop,
                    Options.DefaultName
                );
                continue;
            }
            builder.Services.AddDataRetrieverOptions<ShopDataRetrieverOptions>(
                config,
                shop,
                shop.ToString()
            );
        }
        builder
            .Services.AddOptions<ShopProductProviderOptions>()
            .Bind(config.GetSection($"ShopDataRetrivers:DelayBetweenRequests"))
            .ValidateDataAnnotations()
            .Validate(
                options =>
                    TimeSpan.FromSeconds(3) <= options.DelayBetweenRequests
                    && options.DelayBetweenRequests <= TimeSpan.FromMinutes(1),
                "Delay between requests must fall in range [00:00:03, 00:01:00]"
            )
            .ValidateOnStart();

        var socketsHandler = new SocketsHttpHandler()
        {
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
        };
        builder.Services.AddSingleton<IShopDataRetriever>(sp => new ForaDataRetriever(
            sp.GetRequiredService<IOptionsMonitor<ShopDataRetrieverOptions>>(),
            new HttpClient(socketsHandler)
        ));
        builder.Services.AddSingleton<IShopDataRetriever>(sp => new SilpoDataRetriever(
            sp.GetRequiredService<IOptionsMonitor<ShopDataRetrieverOptions>>(),
            new HttpClient(socketsHandler)
        ));
        builder.Services.AddSingleton<IShopDataRetriever, FozzyDataRetriever>();
        builder.Services.AddSingleton<IShopProductProvider, ShopProductProvider>();
        return builder;
    }

    private static IServiceCollection AddDataRetrieverOptions<T>(
        this IServiceCollection services,
        IConfigurationManager config,
        Shop shop,
        string name
    )
        where T : ShopDataRetrieverOptions
    {
        services
            .AddOptions<T>(name)
            .Bind(config.GetSection($"ShopDataRetrievers:{shop}"))
            .Configure(options =>
            {
                options.RetrieveLimit = config.GetSection("CountOfProductsToRetrieve").Get<int>();
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }
}
