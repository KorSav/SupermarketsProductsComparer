using Microsoft.Extensions.Primitives;

namespace program.Utils;

public static class EnumFromStringConverter
{
    public static void SetFrom<T>(this ref T enumObject, string value)
        where T : struct, Enum
    {
        foreach (var enumValue in Enum.GetValues<T>())
        {
            if (string.Equals(enumValue.ToString(), value, StringComparison.OrdinalIgnoreCase)){
                enumObject = enumValue;
                return;
            }
        }
        throw new ArgumentException($"Cannot convert '{value}' to enum of type '{enumObject.GetType().Name}'");
    }

    public static void SetFromFirstInIfExist<T>(this ref T enumObject, StringValues strings)
        where T : struct, Enum
    {
        if (!StringValues.IsNullOrEmpty(strings) && !string.IsNullOrEmpty(strings[0]))
        {
            enumObject.SetFrom(strings[0]!);
        }
    }
}