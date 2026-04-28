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

    /// <summary>
    /// Returns product with update price and measure up to predefined unit of dimension.
    /// </summary>
    /// <remarks>
    /// To reduce pricision loss, base unit is 1 of biggest unit in given dimension.
    /// This makes sure smaller units after conversion won't have significant value in fractional part.
    /// Precision is lost only if converting N of base unit to 1 base unit and it is accepted since this is rare
    /// </remarks>
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
