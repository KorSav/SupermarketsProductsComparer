namespace ApplicationCore.Entities.Product;

using ApplicationCore.Utils;
using Dim = MeasureDim;
using Unit = MeasureUnit;

public readonly struct Measure
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
        { // to simlify storing, base unit is always biggest possible (not to store many digits after dot on conversion)
            Unit.KiloGram => (Dim.Mass, 1),
            Unit.MiliGram => (Dim.Mass, 1e-6m),
            Unit.Gram => (Dim.Mass, 1e-3m),

            Unit.Litre => (Dim.Volume, 1),
            Unit.MiliLitre => (Dim.Volume, 1e-3m),

            Unit.Meter => (Dim.Length, 1),
            Unit.MiliMeter => (Dim.Length, 1e-3m),
            Unit.SantiMeter => (Dim.Length, 1e-2m),
            Unit.DeciMeter => (Dim.Length, 1e-1m),

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
