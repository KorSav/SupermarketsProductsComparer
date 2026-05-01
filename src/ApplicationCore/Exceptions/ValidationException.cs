namespace ApplicationCore.Exceptions;

using ApplicationCore.Services;
using static ValidationExceptionType;

public enum ValidationExceptionType
{
    StoredRequestsLimitReached = 1,
    TooManyRequest,
    NotAllowed,
}

public static class ValidationExceptionTypeExtension
{
    public static string ToMessage(this ValidationExceptionType type) =>
        type switch
        {
            StoredRequestsLimitReached =>
                $"User can't have more than {StoredRequestsService.MaxLimit} stored requests",
            TooManyRequest =>
                $"Two or more request arrived in parallel but this is forbidden for the action",
            NotAllowed => $"The requested action is not allowed",
            _ => DomainException<ValidationExceptionType>.CatchAllCase(type),
        };
}
