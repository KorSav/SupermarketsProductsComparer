using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PriceComparer.Web.Services.ShopsDataParsing.Attributes;

public class AutoParseJsonConverter<T> : JsonConverter<T>
    where T : class, new()
{
    public override T Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;
        var instance = new T();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<AutoParseAttribute>();
            if (attribute is null)
                continue;
            string jsonPropertyName = attribute.JsonPropertyName ?? property.Name;
            if (root.TryGetProperty(jsonPropertyName, out JsonElement value))
            {
                var parsedValue = JsonSerializer.Deserialize(
                    value.GetRawText(),
                    property.PropertyType,
                    options
                );
                property.SetValue(instance, parsedValue);
            }
            else
            {
                throw new JsonException(
                    $"Missing required property '{jsonPropertyName}' for class '{typeof(T).Name}' in retrieved json."
                );
            }
        }

        return instance;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
