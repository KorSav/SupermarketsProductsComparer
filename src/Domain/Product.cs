using PriceComparer.Domain.Measure;

namespace PriceComparer.Domain;

public class Product<TMeasure>(string name, ProductPrice<TMeasure> productPrice)
    where TMeasure : MagnifiedMeasure
{
    public string Name { get; } = name;

    public ProductPrice<TMeasure> Price { get; } = productPrice;
}
