namespace ApplicationCore.Exceptions;

using static ConflictExceptionType;

public class ConflictException : DomainException
{
    public ConflictExceptionType Type { get; private set; }

    public ConflictException(ConflictExceptionType type)
        : base(MessageFor(type)) { }

    public ConflictException(ConflictExceptionType type, Exception inner)
        : base(MessageFor(type), inner) { }

    public static string MessageFor(ConflictExceptionType type) =>
        type switch
        {
            StoredRequestNotUnique => "Unable to upsert new request into repository.\n"
                + "Possibly concurrency issue with simultaneous requests from the same user",
            UserDuplicateRegister =>
                "Concurrent issue: user with provided credentials has just registered",
            _ => Enum.IsDefined(type)
                ? $"Message is undefined for type: {type}"
                : $"Undefined exception type: {type}",
        };
}

public enum ConflictExceptionType
{
    StoredRequestNotUnique = 1,
    UserDuplicateRegister,
}

public static class ConflictExceptionTypeExtensions
{
    public static ConflictException New(this ConflictExceptionType type) => new(type);

    public static ConflictException New(this ConflictExceptionType type, Exception inner) =>
        new(type, inner);
}
