using PriceComparer.Domain.Measure;

namespace PriceComparer.Domain;

public class ProductPrice<TMeasure>(decimal price, TMeasure measure)
    where TMeasure : MagnifiedMeasure
{
    public decimal Price { get; private set; } = price;
    public TMeasure Measure { get; private set; } = measure;

    public void UpdateMeasure(TMeasure newMeasure)
    {
        decimal ratio = (decimal)(newMeasure.ScaleFactor / Measure.ScaleFactor);
        Price *= ratio;
        Measure = newMeasure;
    }
}
