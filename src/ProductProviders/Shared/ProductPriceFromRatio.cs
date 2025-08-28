using System.Text.RegularExpressions;
using PriceComparer.Domain;
using PriceComparer.ProductProvider.Exceptions;

namespace PriceComparer.ProductProvider.Shared;

internal static partial class ProductPriceFromRatio
{
    [GeneratedRegex(@"((?:\d+\*)?(?:\d+[.,])?\d*)\s*([\p{IsCyrillic}]+)")]
    private static partial Regex RatioRegex();

    /// <exception cref="StringParsingException"></exception>
    public static ProductPrice Convert(decimal price, string ratio)
    {
        var match = RatioRegex().Match(ratio);
        if (!match.Success)
            throw new StringParsingException(
                ratio,
                $"Failed to match ratio with regex: {RatioRegex()}"
            );
        double amount = 1;
        if (!string.IsNullOrWhiteSpace(match.Groups[1].Value))
            amount = EvaluateAmount(match.Groups[1].Value);
        string qualifier = match.Groups[2].Value;
        Measure.Units measureUnit = qualifier switch
        {
            "кг" => Measure.Units.KiloGram,
            "г" => Measure.Units.Gram,
            "л" => Measure.Units.Litre,
            "мл" => Measure.Units.MiliLitre,
            "шт" or "бух" => Measure.Units.Amount, // буханка
            "м" => Measure.Units.Meter,
            "мм" => Measure.Units.MiliMeter,
            "см" => Measure.Units.SantiMeter,
            var q => throw new StringParsingException(
                q,
                "Failed to match existing measure unit with given qualifier"
            ),
        };
        return new(price, amount, measureUnit);
    }

    /// <summary>
    /// Parses amount represented as multiplication of numbers
    /// </summary>
    static double EvaluateAmount(string expression)
    {
        string[] strNumbers = expression.Split('*');
        double result = 1;
        foreach (var strNumber in strNumbers)
        {
            if (!double.TryParse(strNumber, out var num))
                throw new StringParsingException(strNumber, "Failed to parse it as double");
            result *= num;
        }
        return result;
    }
}
