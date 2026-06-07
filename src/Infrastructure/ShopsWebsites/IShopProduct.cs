using ApplicationCore.Entities.Product;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.ShopsWebsites;

public interface IShopProduct
{
    string Name { get; set; }

    decimal Price { get; set; }

    string LinkProduct { get; set; }

    string LinkImage { get; set; }

    string Ratio { get; set; }

    Shop Shop { get; set; }

    Product ToProduct(IConfiguration configuration)
    {
        var baseUrlProduct =
            configuration.GetSection($"ShopDataRetrievers:{Shop}:ProductUrl").Get<string?>()
            ?? string.Empty;
        var baseUrlImage =
            configuration.GetSection($"ShopDataRetrievers:{Shop}:ImageUrl").Get<string?>()
            ?? string.Empty;
        var resNameSufix = ", " + Ratio;
        var resMeasure = RatioParser.Parse(Ratio);
        var resLinkProduct = baseUrlProduct + LinkProduct;
        var resLinkImage = baseUrlImage + LinkImage;
        return new(
            Guid.Empty,
            Name,
            resNameSufix,
            Price,
            resMeasure,
            new Uri(resLinkProduct),
            new Uri(resLinkImage),
            Shop
        );
    }
}
