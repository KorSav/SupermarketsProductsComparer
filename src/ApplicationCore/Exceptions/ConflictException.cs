namespace ApplicationCore.Exceptions;

using static ConflictExceptionType;

public enum ConflictExceptionType
{
    StoredRequestNotUnique = 1,
    UserDuplicateRegister,
}

public static class ConflictExceptionTypeExtension
{
    public static string ToMessage(this ConflictExceptionType type) =>
        type switch
        {
            StoredRequestNotUnique => "Unable to upsert new request into repository.\n"
                + "Possibly concurrency issue with simultaneous requests from the same user",
            UserDuplicateRegister =>
                "Concurrent issue: user with provided credentials has just registered",
            _ => DomainException<ConflictExceptionType>.CatchAllCase(type),
        };
}
