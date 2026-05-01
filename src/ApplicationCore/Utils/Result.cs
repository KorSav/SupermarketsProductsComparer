using System.Diagnostics.CodeAnalysis;

namespace ApplicationCore.Utils;

/// <summary>
/// Value type comparison is not deep, so ErrorList will be compared by reference only
/// </summary>
public record Result<T>
    where T : notnull
{
    public List<Error> ErrorList { get; private set; }

    public T? Value { get; private init; }

    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess => Value is not null;

    internal Result(T value)
    {
        Value = value;
        ErrorList = [];
    }

    internal Result(List<Error> errorList)
    {
        Value = default;
        ErrorList = errorList;
    }

    public static implicit operator Result<T>(T value) => new(value);
};

public static class Result
{
    public static Result<T> Success<T>(T value)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(value);
    }

    public static Result<T> Failure<T>(params IReadOnlyCollection<Error> errors)
        where T : notnull
    {
        if (errors is null || errors.Count == 0)
            throw new ArgumentException("Errors can't be null or empty", nameof(errors));
        return new([.. errors]);
    }

    public static Result<T> Failure<T>(params IReadOnlyCollection<TypedError<T>> errors)
        where T : notnull
    {
        if (errors is null || errors.Count == 0)
            throw new ArgumentException("Errors can't be null or empty", nameof(errors));
        return new([.. errors]);
    }
}
