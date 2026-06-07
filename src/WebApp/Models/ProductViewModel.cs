using System.Text.Json;
using System.Text.Json.Serialization;
using ApplicationCore.Entities.Product;
using WebApp.Mappings;
using static WebApp.Models.ProductViewModel;

namespace WebApp.Models;

public record ProductViewModel(
    List<ProductDetail> Details,
    List<PriceHistory> Chart,
    List<HomeViewModel.ProductInfo> Recommended
)
{
    public string ToJson()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new MeasureConverter());
        options.Converters.Add(new LocalizedShopJsonConverter());
        return JsonSerializer.Serialize(this, options);
    }

    public record ProductDetail(
        Guid Id,
        string DisplayName,
        Shop Shop,
        string LinkImage,
        string LinkProduct,
        decimal Price,
        decimal UnifiedPrice,
        decimal AveragePrice,
        Measure UnifiedMeasure,
        Measure Measure
    );

    public record PriceHistory(Guid ProductDetailId, IReadOnlyCollection<PriceEntry> History)
    {
        public IReadOnlyCollection<PriceEntry> History { get; } =
            History.OrderBy(e => e.Date).ToList().AsReadOnly();
    }

    public record PriceEntry(decimal Price, decimal UnifiedPrice, DateTime Date);

    public record ShopMeasureId(Shop Shop, Measure Measure, Guid Id);

    public List<ShopMeasureId> ShopMeasureIds =>
        Details.Select(d => new ShopMeasureId(d.Shop, d.Measure, d.Id)).ToList();
}

public class MeasureConverter : JsonConverter<Measure>
{
    public override Measure Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Measure value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("Amount", value.Count);
        writer.WriteString("Unit", value.Unit.ToLocalString());
        writer.WriteEndObject();
    }
}

public class LocalizedShopJsonConverter : JsonConverter<Shop>
{
    public override Shop Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Shop value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToLocalString());
    }
}
