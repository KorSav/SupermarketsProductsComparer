namespace PriceComparer.Domain.Measure;

public sealed class Amount : MagnifiedMeasure
{
    public AmountUnit Unit
    {
        get => (AmountUnit)Combine(AppliedMagnifier, BaseUnit.Amount);
        set
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentException($"Invalid amount unit to set: {value}");
            AppliedMagnifier = GetMagnifierFromUnit((byte)value);
        }
    }

    public Amount(double length, AmountUnit unit)
        : base(length, GetMagnifierFromUnit((byte)unit)) { }

    public enum AmountUnit : byte
    {
        Amount = BaseUnit.Amount,
    }
}
