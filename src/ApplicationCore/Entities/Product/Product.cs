namespace ApplicationCore.Entities.Product;

public record Product(
    string Name,
    decimal Price,
    Measure Measure,
    Uri LinkProduct,
    Uri LinkImage,
    Shop Shop
)
{
    public Product WithPricePer(Measure measure)
    {
        var scale = Measure.ScaleFactorTo(measure);
        return this with { Price = Price * scale, Measure = measure };
    }

    public Product WithUnifiedPrice()
    {
        var unifiedMeasure = Measure.Dimension switch
        {
            MeasureDim.Mass => MeasureUnit.KiloGram,
            MeasureDim.Length => MeasureUnit.Meter,
            MeasureDim.Volume => MeasureUnit.Litre,
            MeasureDim.Count => MeasureUnit.Count,
            _ => throw new NotImplementedException(
                $"Unified unit for dimension '{Measure.Dimension}' is undefined"
            ),
        };
        if (Measure.Count is 1 && Measure.Unit == unifiedMeasure)
            return this;
        return WithPricePer(new Measure(1, unifiedMeasure));
    }
}
