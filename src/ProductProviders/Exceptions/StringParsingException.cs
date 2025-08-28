namespace PriceComparer.ProductProvider.Exceptions;

internal class StringParsingException(string convertingValue, string message) : Exception(message)
{
    readonly string _convertingValue = convertingValue;

    public override string ToString()
    {
        return $"""
            Unable to parse a string: {_convertingValue}.
            {base.ToString()}
            """;
    }
}
