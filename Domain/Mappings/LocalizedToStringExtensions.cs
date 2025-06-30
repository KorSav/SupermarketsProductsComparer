using program.Domain.Enums;

namespace program.Domain.Mappings;

public static class LocalizedToStringExtensions
{
    public static string ToLocalString(this Measure measureId) => measureId switch
    {
        Measure.Kg => "кг",
        Measure.L => "л",
        Measure.M => "м",
        Measure.No => "шт",
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