using program.Services.ShopsDataParsing;

namespace program.Domain.Mappings;

public static class ProductMappingExtensions
{
    public static Product ToProduct(this GeneralProduct generalProduct)
    {
        return new Product()
        {
            Name = generalProduct.Name,
            FullLinkImage = generalProduct.FullLinkImage,
            FullLinkProduct = generalProduct.FullLinkProduct,
            ShopId = generalProduct.ShopId,
            MeasureId = generalProduct.MeasureId,
            PriceUnified = generalProduct.PriceUnified,
            PriceInitial = generalProduct.PriceInitial,
            ProductStatusId = Enums.ProductStatusId.Updated
        };
    }

    public static List<Product> ToProducts(this List<GeneralProduct> generalProducts)
    {
        return generalProducts.Select(gp => gp.ToProduct()).ToList();
    }
}