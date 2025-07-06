namespace PriceComparer.Domain.Measure;

public sealed class Volume : MagnifiedMeasure
{
    public VolumeUnit Unit
    {
        get => (VolumeUnit)Combine(AppliedMagnifier, BaseUnit.Litre);
        set
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentException($"Invalid volume unit to set: {value}");
            AppliedMagnifier = GetMagnifierFromUnit((byte)value);
        }
    }

    public Volume(double mass, VolumeUnit unit)
        : base(mass, GetMagnifierFromUnit((byte)unit)) { }

    public enum VolumeUnit
    {
        Litre = BaseUnit.Litre,
        MiliLitre = Magnifier.Mili | Litre,
    }
}
