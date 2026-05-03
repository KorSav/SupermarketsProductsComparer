namespace ApplicationCore.Utils;

public static class Extentions
{
    extension(ArgumentException)
    {
        public static void ThrowIfUndefinedEnum<TEnum>(TEnum value) where TEnum: struct, Enum
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentException(
                    $"Enum {typeof(TEnum).Name} has no definition for value '{value}'"
                );
        }
    }
    
}