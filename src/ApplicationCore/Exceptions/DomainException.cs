namespace ApplicationCore.Exceptions;

public class DomainException<T> : Exception
    where T : struct, Enum
{
    public T Type { get; set; }

    public DomainException(T type)
        : base(MessageFor(type)) => Type = type;

    public DomainException(T type, Exception inner)
        : base(MessageFor(type), inner) => Type = type;

    public static string MessageFor(T type) =>
        type switch
        {
            ValidationExceptionType t => t.ToMessage(),
            ConflictExceptionType t => t.ToMessage(),
            _ => $"Unregistered domain exception type {typeof(T).Name}. "
                + (
                    Enum.IsDefined(type)
                        ? $"Message is undefined for existing enum value: {type}"
                        : $"Value is not defined in exception enum type: {type}"
                ),
        };

    internal static string CatchAllCase(T type) =>
        Enum.IsDefined(type)
            ? $"Message for {typeof(T).Name}.{type} is undefined."
            : $"Value {type} is not defined for enum {typeof(T).Name}";
};

public static class DomainException
{
    public static DomainException<T> For<T>(T type)
        where T : struct, Enum => new(type);

    public static DomainException<T> For<T>(T type, Exception inner)
        where T : struct, Enum => new(type, inner);
}
