using System.Text.RegularExpressions;
using PriceComparer.Domain.Enums;

namespace PriceComparer.Web.Utils;

public static partial class ProductConverter
{
    [GeneratedRegex(@"((?:\d+\*)?(?:\d+[.,])?\d*)\s*([\p{IsCyrillic}a-zA-Z]+)")]
    private static partial Regex RatioRegex();

    public static (decimal price, Measure measure) GeneralizePrice(decimal price, string ratio)
    {
        var match = RatioRegex().Match(ratio);
        if (!match.Success)
            throw new ArgumentException($"Failed to analyze ratio: '{ratio}'");
        decimal amount = 1;
        if (match.Groups[1].Value.Trim() != "")
            amount = EvaluateAmount(match.Groups[1].Value);
        string qualifier = match.Groups[2].Value;
        return qualifier switch
        {
            "кг" => (price / amount, Measure.Kg),
            "г" => (price * 1000 / amount, Measure.Kg),
            "л" => (price / amount, Measure.L),
            "мл" => (price * 1000 / amount, Measure.L),
            "шт" or "бух" => (price / amount, Measure.No), // буханка
            "м" => (price / amount, Measure.M),
            _ => throw new ArgumentException(
                $"Unknown literal '{qualifier}' from '{ratio}' to specify n='{amount}' of m='{qualifier}'"
            ),
        };
    }

    private static decimal EvaluateAmount(string expression)
    {
        string[] strNumbers = expression.Split('*');
        decimal result = 1;
        foreach (var strNumber in strNumbers)
        {
            result *= decimal.Parse(strNumber);
        }
        return result;
    }
}
