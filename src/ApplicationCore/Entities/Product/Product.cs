namespace ApplicationCore.Entities.Product;

public record Product(
    int Id,
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
        var unifiedMeasure = UnifiedMeasure(Measure);
        if (Measure == unifiedMeasure)
            return this;
        return WithPricePer(unifiedMeasure);
    }

    public static Measure UnifiedMeasure(Measure current) =>
        current.Dimension switch
        {
            MeasureDim.Mass => new(1, MeasureUnit.KiloGram),
            MeasureDim.Length => new(1, MeasureUnit.Meter),
            MeasureDim.Volume => new(1, MeasureUnit.Litre),
            MeasureDim.Count => new(1, MeasureUnit.Count),
            _ => throw new NotImplementedException(
                $"Unified unit for dimension '{current.Dimension}' is undefined"
            ),
        };
}
