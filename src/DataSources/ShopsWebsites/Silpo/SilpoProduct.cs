using program.Domain.Enums;
using program.Services.ShopsDataParsing.Attributes;

namespace program.Services.ShopsDataParsing.Silpo;

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

    public ShopId ShopId { get; set; } = ShopId.Silpo;
}