using PriceComparer.Domain.Enums;
using PriceComparer.Web.Utils;

namespace PriceComparer.Web.Services.ShopsDataParsing;

public class ShopProductsGeneralizer
{
    private readonly Dictionary<Shop, string> imageBaseUrls = [];
    private readonly Dictionary<Shop, string> productBaseUrls = [];

    public ShopProductsGeneralizer(IConfiguration configuration)
    {
        foreach (Shop shopId in Enum.GetValues(typeof(Shop)))
        {
            string shopName = Enum.GetName(typeof(Shop), shopId)!;
            imageBaseUrls.Add(
                shopId,
                configuration.GetSection($"ShopDataRetrievers:{shopName}:ImageUrl").Get<string?>()
                    ?? string.Empty
            );
            productBaseUrls.Add(
                shopId,
                configuration.GetSection($"ShopDataRetrievers:{shopName}:ProductUrl").Get<string?>()
                    ?? string.Empty
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
                PriceInitial = shopProduct.Price,
            };
            try
            {
                (generalProduct.PriceUnified, generalProduct.MeasureId) =
                    ProductConverter.GeneralizePrice(shopProduct.Price, shopProduct.Ratio);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                continue;
            }
            if (shopProduct.ShopId == Shop.Fozzy)
                Console.WriteLine("Fozzy product");
            generalProduct.FullLinkImage =
                imageBaseUrls[shopProduct.ShopId] + shopProduct.LinkImage;
            generalProduct.FullLinkProduct =
                productBaseUrls[shopProduct.ShopId] + shopProduct.LinkProduct;
            generalProduct.ShopId = shopProduct.ShopId;
            generalProducts.Add(generalProduct);
        }
        return generalProducts;
    }
}
