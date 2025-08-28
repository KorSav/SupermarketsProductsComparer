using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PriceComparer.Application.Products.Provider;
using PriceComparer.Application.Products.Types;
using PriceComparer.ProductProvider.Shared;

namespace PriceComparer.ProductProvider;

public class ProductProviderFactory : IProductProviderFactory
{
    internal Lazy<IConfiguration> shopsConfig = new(
        () =>
            new ConfigurationBuilder()
                .AddJsonFile("libsettings.json", optional: false, reloadOnChange: true)
                .Build()
    );

    public IProductProvider Create(Shop shop, HttpClient client, ILoggerFactory loggerFactory)
    {
        if (!Enum.IsDefined(shop))
            throw new ArgumentException($"Shop by number {shop} does not exist", nameof(shop));
        string section = shop switch
        {
            Shop.Fora => "Fora",
            Shop.Silpo => "Silpo",
            Shop.Fozzy => "Fozzy",
            var nsShop => throw new NotSupportedException(
                $"Config file does not support shop '{nsShop}'"
            ),
        };
        var config =
            shopsConfig.Value.GetRequiredSection(section).Get<RTConfig>()
            ?? throw new ArgumentException(
                $"Invalid config file. Expected to have {typeof(RTConfig).FullName} object in '{section}' section"
            );
        IProductByNameProvider provider = shop switch
        {
            Shop.Fora => CreateForaProvider(client, loggerFactory, config.MaxProdCountPerRequest),
            Shop.Silpo => CreateSilpoProvider(client, loggerFactory, config.MaxProdCountPerRequest),
            Shop.Fozzy => CreateFozzyProvider(
                client,
                loggerFactory,
                config.MaxProdCountPerRequest,
                config.PaginationDelay
            ),
            var nsShop => throw new NotSupportedException(
                $"Failed to create provider for shop '{nsShop}'"
            ),
        };
        return new DelayBetweenRequestsProvider(
            provider,
            SharedRequests.Requests,
            config.DelayBetweenRequests
        );
    }

    public IEnumerable<IProductProvider> CreateAll(HttpClient client, ILoggerFactory loggerFactory)
    {
        var shops = Enum.GetValues<Shop>();
        return [.. shops.Select(s => Create(s, client, loggerFactory))];
    }

    static Fozzy.ProductProvider CreateFozzyProvider(
        HttpClient client,
        ILoggerFactory loggerFactory,
        int MaxProdCount,
        TimeSpan paginationDelay
    )
    {
        ProductProviderConfig config = new(
            client,
            StaticConfig.FozzyUriBuilder,
            StaticConfig.FozzyQueryParams,
            MaxProdCount
        );
        return new Fozzy.ProductProvider(
            config,
            paginationDelay,
            loggerFactory.CreateLogger<Fozzy.ProductProvider>()
        );
    }

    static Fora.ProductProvider CreateForaProvider(
        HttpClient client,
        ILoggerFactory loggerFactory,
        int MaxProdCount
    )
    {
        ProductProviderConfig config = new(client, StaticConfig.ForaUriBuilder, [], MaxProdCount);
        return new Fora.ProductProvider(
            config,
            StaticConfig.ForaLinkPrefixes,
            loggerFactory.CreateLogger<Fora.ProductProvider>()
        );
    }

    static Silpo.ProductProvider CreateSilpoProvider(
        HttpClient client,
        ILoggerFactory loggerFactory,
        int MaxProdCount
    )
    {
        ProductProviderConfig config = new(
            client,
            StaticConfig.SilpoUriBuilder,
            StaticConfig.SilpoQueryParams,
            MaxProdCount
        );
        return new Silpo.ProductProvider(
            config,
            StaticConfig.SilpoLinkPrefixes,
            loggerFactory.CreateLogger<Silpo.ProductProvider>()
        );
    }
}
