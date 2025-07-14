namespace PriceComparer.Domain;

public class ProductPrice(decimal price, double amount, Measure measure)
{
    public decimal Price { get; private set; } = price;
    public double Amount { get; private set; } = amount;
    public Measure Measure { get; private set; } = measure;

    public ProductPrice AsUnified() =>
        new(Measure.Unify(Price), Measure.Unify(Amount), Measure.Unify());
}
