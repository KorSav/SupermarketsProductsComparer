using PriceComparer.Domain.Enums;
using PriceComparer.Web.Services.ShopsDataParsing.Attributes;

namespace PriceComparer.Web.Services.ShopsDataParsing.Silpo;

public class SilpoProduct : IShopProduct
{
    [AutoParse("title")]
    public string Name { get; set; } = null!;

    [AutoParse("displayPrice")]
    public decimal Price { get; set; }

    [AutoParse("slug")]
    public string LinkProduct { get; set; } = null!;

    [AutoParse("icon")]
    public string LinkImage { get; set; } = null!;

    [AutoParse("displayRatio")]
    public string Ratio { get; set; } = null!;

    public Shop ShopId { get; set; } = Shop.Silpo;
}