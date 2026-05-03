namespace Infrastructure.ShopsWebsites.Exceptions;

public class ConversionException : Exception
{
    public ConversionException(string value, Type type)
        : base($"Failed to convert '{value}' into {type.Name}") { }
}
