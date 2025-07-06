namespace PriceComparer.Domain.Measure;

public sealed class Mass : MagnifiedMeasure
{
    public MassUnit Unit
    {
        get => (MassUnit)Combine(AppliedMagnifier, BaseUnit.Gram);
        set
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentException($"Invalid mass unit to set: {value}");
            AppliedMagnifier = GetMagnifierFromUnit((byte)value);
        }
    }

    public Mass(double mass, MassUnit unit)
        : base(mass, GetMagnifierFromUnit((byte)unit)) { }

    public enum MassUnit
    {
        Gram = BaseUnit.Gram,
        KiloGram = Magnifier.Kilo | Gram,
        MiliGram = Magnifier.Mili | Gram,
    }
}
