using System.Text.RegularExpressions;
using ApplicationCore.Entities.Product;

namespace Infrastructure.ShopsWebsites;

public static partial class RatioParser
{
    [GeneratedRegex(@"((?:\d+\*)?(?:\d+[.,])?\d*)\s*([\p{IsCyrillic}a-zA-Z]+)")]
    private static partial Regex RatioRegex();

    public static Measure Parse(string ratio)
    {
        var match = RatioRegex().Match(ratio);
        if (!match.Success)
            throw new ArgumentException($"Failed to parse ratio: '{ratio}'");
        decimal amount = 1;
        if (match.Groups[1].Value.Trim() != "")
            amount = EvaluateAmount(match.Groups[1].Value);
        var unitStr = match.Groups[2].Value;
        MeasureUnit unit = unitStr switch
        {
            "кг" => MeasureUnit.KiloGram,
            "г" => MeasureUnit.Gram,
            "л" => MeasureUnit.Litre,
            "мл" => MeasureUnit.MiliLitre,
            "шт" or "бух" => MeasureUnit.Count, // буханка
            "м" => MeasureUnit.Meter,
            _ => throw new ArgumentException(
                $"Unknown literal '{unitStr}' from '{ratio}' to specify amount '{amount}' of '{unitStr}' qualifier"
            ),
        };
        return new(amount, unit);
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
