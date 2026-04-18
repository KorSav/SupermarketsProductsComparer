namespace program.Services.ShopsDataParsing.Attributes;


[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public class AutoParseAttribute(string jsonPropertyName) : Attribute
{
    public string JsonPropertyName { get; set; } = jsonPropertyName;
}