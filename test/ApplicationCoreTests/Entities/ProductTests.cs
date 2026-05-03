using ApplicationCore.Entities.Product;
using Unit = ApplicationCore.Entities.Product.MeasureUnit;

namespace ApplicationCoreTests.Entities;

public class ProductTests
{
    [Theory]
    [MemberData(nameof(ScaleFactorsData))]
    public void ScaleFactorTo_ReturnsExpectedScale(Measure from, Measure to, decimal exp)
    {
        Assert.Equal(exp, from.ScaleFactorTo(to));
    }

    public static IEnumerable<TheoryDataRow<Measure, Measure, decimal>> ScaleFactorsData() =>
        [
            // mass
            (new(1, Unit.Gram), new(20, Unit.Gram), 20),
            (new(50, Unit.Gram), new(10, Unit.Gram), 0.2m),
            (new(1, Unit.Gram), new(1, Unit.KiloGram), 1000),
            (new(200, Unit.Gram), new(1, Unit.KiloGram), 5),
            (new(250, Unit.MiliGram), new(1, Unit.Gram), 4),
            (new(5000, Unit.MiliGram), new(1, Unit.KiloGram), 200),
            (new(1, Unit.KiloGram), new(200, Unit.Gram), 0.2m),
            // length
            (new(1, Unit.MiliMeter), new(1, Unit.SantiMeter), 10),
            (new(1, Unit.SantiMeter), new(1, Unit.DeciMeter), 10),
            (new(1, Unit.DeciMeter), new(1, Unit.Meter), 10),
            (new(1, Unit.MiliMeter), new(1, Unit.Meter), 1000),
            (new(12.5m, Unit.SantiMeter), new(2, Unit.Meter), 16),
            (new(15, Unit.DeciMeter), new(3, Unit.Meter), 2),
            (new(1, Unit.Meter), new(70, Unit.MiliMeter), 0.07m),
            // volume
            (new(1, Unit.MiliLitre), new(1, Unit.Litre), 1000),
            (new(250, Unit.MiliLitre), new(1, Unit.Litre), 4),
            (new(3, Unit.Litre), new(300, Unit.MiliLitre), 0.1m),
            // amount
            (new(1, Unit.Count), new(10, Unit.Count), 10),
            (new(7, Unit.Count), new(56, Unit.Count), 8),
            (new(24, Unit.Count), new(6, Unit.Count), 0.25m),
        ];
}
