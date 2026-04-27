using program.Domain.Entities.Product;

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

    public static string ToLocalString(this Shop shopId) => shopId switch
    {
        Shop.Silpo => "Сільпо",
        Shop.Fora => "Фора",
        Shop.Fozzy => "Фоззі",
        _ => throw new NotImplementedException()
    };
}