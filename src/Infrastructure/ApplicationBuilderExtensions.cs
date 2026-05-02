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
        builder.Services.AddDbContext<AppDbContext>(options =>
            options
                .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        );
        builder
            .Services.AddScoped<IProductRepository, ProductRepository>()
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
            .Configure(o =>
                o.DelayBetweenRequests = config
                    .GetSection("ShopDataRetrievers:DelayBetweenRequests")
                    .Get<TimeSpan>()
            )
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddHttpClient<IShopDataRetriever, ForaDataRetriever>();
        builder.Services.AddHttpClient<IShopDataRetriever, SilpoDataRetriever>();
        builder.Services.AddScoped<IShopDataRetriever, FozzyDataRetriever>();
        builder.Services.AddScoped<IShopProductProvider, ShopProductProvider>();
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
