using System.Globalization;
using program.Models;
using program.Services.ShopsDataParsing;
using program.Utils;

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

    public static ProductViewModel ToProductViewModel(this Product product){
        return new ProductViewModel(
            Name: product.Name,
            Price: product.PriceInitial.ToString("N"),
            PriceUnified: product.PriceUnified.ToString("N"),
            ProductLink: product.FullLinkProduct,
            ImageLink: product.FullLinkImage,
            MeasureUnified: product.MeasureId.ToLocalString(),
            ShopName: product.ShopId.ToLocalString()
        );
    }

    public static List<Product> ToProducts(this List<GeneralProduct> generalProducts)
    {
        return generalProducts.Select(gp => gp.ToProduct()).ToList();
    }

    public static List<ProductViewModel> ToProductViewModels(this List<Product> products){
        return products.Select(p => p.ToProductViewModel()).ToList();
    }

    public static PaginatedList<ProductViewModel> ToProductViewModels(this PaginatedList<Product> products){
        return products.Map(p => p.ToProductViewModel());
    }
}