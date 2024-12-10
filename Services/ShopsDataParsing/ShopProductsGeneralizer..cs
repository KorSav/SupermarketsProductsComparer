using program.Services.ShopsDataParsing.Enums;
using program.Utils;

namespace program.Services.ShopsDataParsing;

public class ShopProductsGeneralizer
{
    private readonly Dictionary<ShopType, string> imageBaseUrls = [];
    private readonly Dictionary<ShopType, string> productBaseUrls = [];

    public ShopProductsGeneralizer(IConfiguration configuration)
    {
        foreach (ShopType shopType in Enum.GetValues(typeof(ShopType)))
        {
            string shopName = Enum.GetName(typeof(ShopType), shopType)!;
            imageBaseUrls.Add(
                shopType,
                configuration.GetSection($"ShopDataRetrievers:{shopName}:ImageUrl")
                    .Get<string?>() ?? string.Empty
            );
            productBaseUrls.Add(
                shopType,
                configuration.GetSection($"ShopDataRetrievers:{shopName}:ProductUrl")
                    .Get<string?>() ?? string.Empty
            );
        }
    }

    public List<GeneralProduct> Generalize(List<IShopProduct> shopProducts)
    {
        List<GeneralProduct> generalProducts = new(shopProducts.Count);
        foreach (IShopProduct shopProduct in shopProducts)
        {
            GeneralProduct generalProduct = new()
            {
                Name = shopProduct.Name
            };
            (generalProduct.Price, generalProduct.Measure) = ProductConverter
                .GeneralizePrice(shopProduct.Price, shopProduct.Ratio);
            generalProduct.FullLinkImage = imageBaseUrls[shopProduct.Shop] + shopProduct.LinkImage;
            generalProduct.FullLinkProduct = productBaseUrls[shopProduct.Shop] + shopProduct.LinkProduct;
            generalProduct.Shop = shopProduct.Shop;
            generalProducts.Add(generalProduct);
        }
        return generalProducts;
    }
}