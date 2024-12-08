using program.Services.ShopsDataParsing.Enums;

namespace program.Services.ShopsDataParsing;

public class GeneralProduct
{
    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string FullLinkProduct { get; set; } = null!;

    public string FullLinkImage { get; set; } = null!;

    public MeasureType Measure { get; set; }

    public ShopType Shop { get; set; }
}