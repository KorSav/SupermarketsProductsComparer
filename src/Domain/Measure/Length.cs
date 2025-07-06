namespace PriceComparer.Domain.Measure;

public sealed class Length : MagnifiedMeasure
{
    public LengthUnit Unit
    {
        get => (LengthUnit)Combine(AppliedMagnifier, BaseUnit.Meter);
        set
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentException($"Invalid length unit to set: {value}");
            AppliedMagnifier = GetMagnifierFromUnit((byte)value);
        }
    }

    public Length(double length, LengthUnit unit)
        : base(length, GetMagnifierFromUnit((byte)unit)) { }

    public enum LengthUnit : byte
    {
        Meter = BaseUnit.Meter,
        SantiMeter = Magnifier.Santi | Meter,
        MiliMeter = Magnifier.Mili | Meter,
    }
}
