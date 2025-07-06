namespace PriceComparer.Domain.Measure;

public abstract partial class MagnifiedMeasure
{
    // If change underlying values try to keep backward compatibility
    private protected enum BaseUnit : byte
    {
        Gram = 1,
        Litre = 2,
        Meter = 3,
        Amount = 4,
    }

    const byte _magnifierBits = 4;
    const byte _magnifierMask = 0b1111_0000;

    // If change underlying values try to keep backward compatibility
    private protected enum Magnifier : byte
    {
        Absent = 0x0 << _magnifierBits, // never change since should: BaseUnit == Absent | BaseUnit
        Kilo = 0x1 << _magnifierBits,
        Mili = 0x2 << _magnifierBits,
        Santi = 0x3 << _magnifierBits,
    }
}
