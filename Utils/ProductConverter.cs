using System.Text.RegularExpressions;
using program.Services.ShopsDataParsing.Enums;

namespace program.Utils;

public static partial class ProductConverter
{
    [GeneratedRegex(@"([+-]?(?:[0-9]*[.,])?[0-9]*)([\p{IsCyrillic}a-zA-Z]*)")]
    private static partial Regex RatioRegex();
    public static (decimal price, MeasureType measure) GeneralizePrice(decimal price, string ratio)
    {
        var match = RatioRegex().Match(ratio);
        if (!match.Success) throw new ArgumentException($"Failed to analyze ratio: {ratio}");
        if (!decimal.TryParse(match.Groups[1].Value, out decimal amount))
            amount = 1;
        string qualifier = match.Groups[2].Value;
        return qualifier switch
        {
            "кг" => (price / amount, MeasureType.Kilogram),
             "г" => (price * 1000 / amount, MeasureType.Kilogram),
             "л" => (price / amount, MeasureType.Litre),
             "мл" => (price * 1000 / amount, MeasureType.Litre),
             "шт" => (price / amount, MeasureType.Number),
             _ => throw new ArgumentException($"Unknown literal {qualifier}: {amount} of {qualifier}")
        };
    }
}
