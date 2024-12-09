using System.Data;
using System.Text.RegularExpressions;
using program.Services.ShopsDataParsing.Enums;

namespace program.Utils;

public static partial class ProductConverter
{
    [GeneratedRegex(@"((?:\d+\*)?(?:\d+[.,])?\d*)([\p{IsCyrillic}a-zA-Z]+)")]
    private static partial Regex RatioRegex();
    public static (decimal price, MeasureType measure) GeneralizePrice(decimal price, string ratio)
    {
        var match = RatioRegex().Match(ratio);
        if (!match.Success) throw new ArgumentException($"Failed to analyze ratio: '{ratio}'");
        decimal amount = 1;
        if (match.Groups[1].Value.Trim() != "")
            amount = EvaluateAmount(match.Groups[1].Value);
        string qualifier = match.Groups[2].Value;
        return qualifier switch
        {
            "кг" => (price / amount, MeasureType.Kilogram),
            "г" => (price * 1000 / amount, MeasureType.Kilogram),
            "л" => (price / amount, MeasureType.Litre),
            "мл" => (price * 1000 / amount, MeasureType.Litre),
            "шт" => (price / amount, MeasureType.Number),
            _ => throw new ArgumentException($"Unknown literal '{qualifier}' from '{ratio}' to specify n='{amount}' of m='{qualifier}'")
        };
    }
    
    private static decimal EvaluateAmount(string expression){
        string[] strNumbers = expression.Split('*');
        decimal result = 1;
        foreach (var strNumber in strNumbers){
            result *= decimal.Parse(strNumber);
        }
        return result;
    }
}
