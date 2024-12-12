using System.Text.RegularExpressions;
using program.Domain.Enums;

namespace program.Utils;

public static partial class ProductConverter
{
    [GeneratedRegex(@"((?:\d+\*)?(?:\d+[.,])?\d*)([\p{IsCyrillic}a-zA-Z]+)")]
    private static partial Regex RatioRegex();
    public static (decimal price, MeasureId measure) GeneralizePrice(decimal price, string ratio)
    {
        var match = RatioRegex().Match(ratio);
        if (!match.Success) throw new ArgumentException($"Failed to analyze ratio: '{ratio}'");
        decimal amount = 1;
        if (match.Groups[1].Value.Trim() != "")
            amount = EvaluateAmount(match.Groups[1].Value);
        string qualifier = match.Groups[2].Value;
        return qualifier switch
        {
            "кг" => (price / amount, MeasureId.Kg),
            "г" => (price * 1000 / amount, MeasureId.Kg),
            "л" => (price / amount, MeasureId.L),
            "мл" => (price * 1000 / amount, MeasureId.L),
            "шт" or "бух" => (price / amount, MeasureId.No), // буханка
            "м" => (price / amount, MeasureId.M),
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
