using program.Domain.Enums;

namespace program.Domain.Mappings;

public static class LocalizedToStringExtensions
{
    public static string ToLocalString(this MeasureId measureId) => measureId switch
    {
        MeasureId.Kg => "кг",
        MeasureId.L => "л",
        MeasureId.M => "м",
        MeasureId.No => "шт",
        _ => throw new NotImplementedException()
    };

    public static string ToLocalString(this ShopId shopId) => shopId switch
    {
        ShopId.Silpo => "Сільпо",
        ShopId.Fora => "Фора",
        ShopId.Fozzy => "Фоззі",
        _ => throw new NotImplementedException()
    };
}