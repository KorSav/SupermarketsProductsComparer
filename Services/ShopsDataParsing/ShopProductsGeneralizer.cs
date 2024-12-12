using program.Domain.Enums;
using program.Utils;

namespace program.Services.ShopsDataParsing;

public class ShopProductsGeneralizer
{
    private readonly Dictionary<ShopId, string> imageBaseUrls = [];
    private readonly Dictionary<ShopId, string> productBaseUrls = [];

    public ShopProductsGeneralizer(IConfiguration configuration)
    {
        foreach (ShopId shopId in Enum.GetValues(typeof(ShopId)))
        {
            string shopName = Enum.GetName(typeof(ShopId), shopId)!;
            imageBaseUrls.Add(
                shopId,
                configuration.GetSection($"ShopDataRetrievers:{shopName}:ImageUrl")
                    .Get<string?>() ?? string.Empty
            );
            productBaseUrls.Add(
                shopId,
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
                Name = shopProduct.Name + ", " + shopProduct.Ratio,
                PriceInitial = shopProduct.Price
            };
            (generalProduct.PriceUnified, generalProduct.MeasureId) = ProductConverter
                .GeneralizePrice(shopProduct.Price, shopProduct.Ratio);
            generalProduct.FullLinkImage = imageBaseUrls[shopProduct.ShopId] + shopProduct.LinkImage;
            generalProduct.FullLinkProduct = productBaseUrls[shopProduct.ShopId] + shopProduct.LinkProduct;
            generalProduct.ShopId = shopProduct.ShopId;
            generalProducts.Add(generalProduct);
        }
        return generalProducts;
    }
}