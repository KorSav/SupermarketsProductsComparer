namespace ApplicationCore.Exceptions;

using ApplicationCore.Services;
using static ValidationExceptionType;

public class ValidationException : DomainException
{
    public ValidationExceptionType Type { get; private set; }

    public ValidationException(ValidationExceptionType type)
        : base(MessageFor(type)) { }

    public ValidationException(ValidationExceptionType type, Exception inner)
        : base(MessageFor(type), inner) { }

    public static string MessageFor(ValidationExceptionType type) =>
        type switch
        {
            StoredRequestsLimitExceeded =>
                $"User can't have more that {StoredRequestsService.MaxLimit} stored request",
            _ => Enum.IsDefined(type)
                ? $"Message is undefined for type: {type}"
                : $"Undefined exception type: {type}",
        };
}

public enum ValidationExceptionType
{
    StoredRequestsLimitExceeded = 1,
}

public static class ValidationExceptionTypeExtensions
{
    public static ValidationException New(this ValidationExceptionType type) => new(type);

    public static ValidationException New(this ValidationExceptionType type, Exception inner) =>
        new(type, inner);
}
