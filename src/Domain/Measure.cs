namespace PriceComparer.Domain;

public readonly struct Measure
{
    public readonly Units Unit { get; }

    public Measure(Units unit)
    {
        if (!Enum.IsDefined(unit))
            throw new ArgumentException($"Invalid measure unit to set: {unit}");
        Unit = unit;
    }

    public Measure Unify() => new(GetUnified(Unit));

    /// <summary>
    /// Multiply by this value to transform some value from current unit to unified one
    /// </summary>
    public double UnifyRatio() => GetScaleFactor(GetUnified(Unit)) / GetScaleFactor(Unit);

    static Units GetUnified(Units unit)
    {
        BaseUnit baseUnit = BaseUnitFrom(unit);
        return baseUnit switch
        {
            BaseUnit.Gram => Units.KiloGram,
            BaseUnit.Litre or BaseUnit.Meter or BaseUnit.Amount => (Units)baseUnit,
            _ => throw new ArgumentOutOfRangeException(
                $"Unsupported base unit code was given: {baseUnit}"
            ),
        };
    }

    static double GetScaleFactor(Units unit) =>
        MagnifierFrom(unit) switch
        {
            Magnifier.Absent => 1,
            Magnifier.Kilo => 1e3,
            Magnifier.Mili => 1e-3,
            Magnifier.Santi => 1e-2,
            _ => throw new ArgumentOutOfRangeException(
                $"Unsupported magnifier code was given: {unit}"
            ),
        };

    static BaseUnit BaseUnitFrom(Units unit) => (BaseUnit)(byte)((byte)unit & ~_magnifierMask);

    static Magnifier MagnifierFrom(Units unit) => (Magnifier)(byte)((byte)unit & _magnifierMask);

    public enum Units : byte
    {
        Amount = BaseUnit.Amount,
        Meter = BaseUnit.Meter,
        SantiMeter = Magnifier.Santi | Meter,
        MiliMeter = Magnifier.Mili | Meter,
        Gram = BaseUnit.Gram,
        KiloGram = Magnifier.Kilo | Gram,
        MiliGram = Magnifier.Mili | Gram,
        Litre = BaseUnit.Litre,
        MiliLitre = Magnifier.Mili | Litre,
    }

    // If change underlying values try to keep backward compatibility
    enum BaseUnit : byte
    {
        Gram = 1,
        Litre = 2,
        Meter = 3,
        Amount = 4,
    }

    const byte _magnifierBits = 4;
    const byte _magnifierMask = 0b1111_0000;

    // If change underlying values try to keep backward compatibility
    enum Magnifier : byte
    {
        Absent = 0x0 << _magnifierBits, // never change since should: BaseUnit == (Absent | BaseUnit)
        Kilo = 0x1 << _magnifierBits,
        Mili = 0x2 << _magnifierBits,
        Santi = 0x3 << _magnifierBits,
    }
}
