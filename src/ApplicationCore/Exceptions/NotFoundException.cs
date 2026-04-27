namespace ApplicationCore.Exceptions;

using static NotFoundExceptionType;

public class NotFoundException : DomainException
{
    public NotFoundExceptionType Type { get; private set; }

    public NotFoundException(NotFoundExceptionType type)
        : base(MessageFor(type)) { }

    public NotFoundException(NotFoundExceptionType type, Exception inner)
        : base(MessageFor(type), inner) { }

    public static string MessageFor(NotFoundExceptionType type) =>
        type switch
        {
            StoredRequestDoesNotExist => "User's stored request was not present in db.\n"
                + "Either malformed id or concurrency issue with simultaneous requests from the same user",
            _ => Enum.IsDefined(type)
                ? $"Message is undefined for type: {type}"
                : $"Undefined exception type: {type}",
        };
}

public enum NotFoundExceptionType
{
    StoredRequestDoesNotExist = 1,
}

public static class NotFoundExceptionTypeExtensions
{
    public static NotFoundException New(this NotFoundExceptionType type) => new(type);

    public static NotFoundException New(this NotFoundExceptionType type, Exception inner) =>
        new(type, inner);
}
