namespace ApplicationCore.Entities.Product;

using ApplicationCore.Utils;
using Dim = MeasureDim;
using Unit = MeasureUnit;

public readonly record struct Measure
{
    public readonly Unit Unit { get; }
    public readonly Dim Dimension { get; }
    public readonly decimal Count { get; }
    private readonly decimal _scale;

    public Measure(decimal count, Unit unit)
    {
        ArgumentException.ThrowIfUndefinedEnum(unit);
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);

        Unit = unit;
        Count = count;
        (Dimension, _scale) = unit switch
        {
            Unit.KiloGram => (Dim.Mass, 1),
            Unit.MiliGram => (Dim.Mass, 1e6m),
            Unit.Gram => (Dim.Mass, 1e3m),

            Unit.Litre => (Dim.Volume, 1),
            Unit.MiliLitre => (Dim.Volume, 1e3m),

            Unit.Meter => (Dim.Length, 1),
            Unit.MiliMeter => (Dim.Length, 1e3m),
            Unit.SantiMeter => (Dim.Length, 1e2m),
            Unit.DeciMeter => (Dim.Length, 10),

            Unit.Count => (Dim.Count, 1),
            _ => throw new NotImplementedException(
                $"Dimension and scale are not defined for unit '{unit}'"
            ),
        };
    }

    /// <summary>
    /// Returns multiplier that must be applied to left measure to get right
    /// </summary>
    public decimal ScaleFactorTo(Measure measure)
    {
        var scaleUnit = _scale / measure._scale;
        var scaleAmount = measure.Count / Count;
        return scaleUnit * scaleAmount;
    }
}

public enum MeasureDim
{
    Mass = 1,
    Volume,
    Length,
    Count,
}

public enum MeasureUnit
{
    Gram = 1,
    KiloGram,
    MiliGram,

    Litre,
    MiliLitre,

    Meter,
    MiliMeter,
    SantiMeter,
    DeciMeter,

    Count,
}
