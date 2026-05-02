using System.Globalization;

namespace ApplicationCoreTests;

public class DecimalFormatTests
{
    [Theory]
    [MemberData(nameof(Data_ToPriceString_HasAtMost2PlacesAfterDot))]
    public void ToPriceString_HasAtMost2PlacesAfterDot(decimal value, string exp) //TODO
    {
        Assert.Equal(value.ToString("#,##0.##", CultureInfo.InvariantCulture), exp);
    }

    public static TheoryData<decimal, string> Data_ToPriceString_HasAtMost2PlacesAfterDot() =>
        [
            (1.3456m, "1.35"),
            (1234.567m, "1,234.57"),
            (1.0000m, "1"),
            (0001.000m, "1"),
            (1_234_567.23456m, "1,234,567.23"),
            (1_234_567.00000m, "1,234,567"),
        ];
}
