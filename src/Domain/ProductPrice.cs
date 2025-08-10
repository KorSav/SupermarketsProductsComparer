namespace PriceComparer.Domain;

public class ProductPrice
{
    public decimal Price { get; private set; }
    public double Amount { get; private set; }
    public Measure Measure { get; private set; }

    public ProductPrice(decimal price, double amount, Measure.Units measureUnit)
        : this(price, amount, new Measure(measureUnit)) { }

    ProductPrice(decimal price, double amount, Measure measure)
    {
        Price = price;
        Amount = amount;
        Measure = measure;
    }

    public ProductPrice ToUnified()
    {
        decimal unifiedPrice = Price * (decimal)Measure.UnifyRatio();
        double unifiedAmount = Amount * Measure.UnifyRatio();
        return new(unifiedPrice, unifiedAmount, Measure.Unify());
    }
}
