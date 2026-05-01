namespace ApplicationCore.Exceptions;

using static NotFoundExceptionType;

public enum NotFoundExceptionType
{
    StoredRequestDoesNotExist = 1,
    UserDoesNotExist,
}

public static class NotFoundExceptionTypeExtension
{
    public static string ToMessage(this NotFoundExceptionType type) =>
        type switch
        {
            StoredRequestDoesNotExist => "User's stored request was not present in db.\n"
                + "Either malformed id or concurrency issue with simultaneous requests from the same user",
            UserDoesNotExist => "There is not requested user. Possibly concurrency issue",
            _ => DomainException<NotFoundExceptionType>.CatchAllCase(type),
        };
}
