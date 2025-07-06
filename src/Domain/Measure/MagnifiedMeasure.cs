namespace PriceComparer.Domain.Measure;

public abstract partial class MagnifiedMeasure
{
    public double Value { get; private set; }
    public double ScaleFactor => GetScaleFactor(_magnifier);
    Magnifier _magnifier;
    private protected Magnifier AppliedMagnifier
    {
        get => _magnifier;
        set
        {
            double valueNoMagnifier = Value / GetScaleFactor(_magnifier);
            Value = valueNoMagnifier * GetScaleFactor(value);
            _magnifier = value;
        }
    }

    private protected MagnifiedMeasure(double value, Magnifier magnifier)
    {
        Value = value;
        AppliedMagnifier = magnifier;
    }

    private protected static Magnifier GetMagnifierFromUnit(byte unit) =>
        (Magnifier)(byte)(unit & _magnifierMask);

    private protected static byte Combine(Magnifier magnifier, BaseUnit baseUnit) =>
        (byte)((byte)magnifier | (byte)baseUnit);

    static double GetScaleFactor(Magnifier m) =>
        m switch
        {
            Magnifier.Absent => 1,
            Magnifier.Kilo => 1e3,
            Magnifier.Mili => 1e-3,
            Magnifier.Santi => 1e-2,
            _ => throw new ArgumentOutOfRangeException(
                $"Unsupported magnifier code was given: {m}"
            ),
        };
}
