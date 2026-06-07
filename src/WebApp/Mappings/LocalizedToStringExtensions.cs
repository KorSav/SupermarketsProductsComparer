using System.Globalization;
using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;

namespace WebApp.Mappings;

public static class LocalizedToStringExtensions
{
    public static string ToLocalString(this MeasureUnit unit) =>
        unit switch
        {
            MeasureUnit.KiloGram => "кг",
            MeasureUnit.Gram => "г",
            MeasureUnit.MiliGram => "мг",
            MeasureUnit.Litre => "л",
            MeasureUnit.MiliLitre => "мл",
            MeasureUnit.Meter => "м",
            MeasureUnit.MiliMeter => "мм",
            MeasureUnit.SantiMeter => "см",
            MeasureUnit.DeciMeter => "дм",
            MeasureUnit.Count => "шт",
            _ => throw new NotImplementedException($"Missing translation for: {unit}"),
        };

    public static string ToLocalString(this Shop shop) =>
        shop switch
        {
            Shop.Silpo => "Сільпо",
            Shop.Fora => "Фора",
            Shop.Fozzy => "Фоззі",
            _ => throw new NotImplementedException(),
        };

    public static string ToLocalString(this SortBy sortBy) =>
        sortBy switch
        {
            SortBy.Name => "назвою",
            SortBy.UnifiedPrice => "загальною ціною",
            SortBy.Price => "ціною",
            _ => throw new NotImplementedException(),
        };

    public static string ToDisplayString(this decimal value) =>
        value.ToString("#,##0.##", CultureInfo.InvariantCulture);
}
