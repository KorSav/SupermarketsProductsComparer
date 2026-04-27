using System.Diagnostics.CodeAnalysis;

namespace ApplicationCore.Utils;

public record Result<T>
    where T : notnull
{
    public List<Error> ErrorList { get; private set; } = [];
    private readonly T? _value;

    private Result(T? value)
    {
        _value = value;
    }

    private Result()
    {
        _value = default;
    }

    public static Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure(params IReadOnlyCollection<Error> errors)
    {
        if (errors is null || errors.Count == 0)
            throw new ArgumentException("Errors can't be null or empty", nameof(errors));
        return new Result<T>() { ErrorList = [.. errors] };
    }

    public bool TryGetValue([NotNullWhen(true)] out T? value)
    {
        value = default;
        if (_value is null)
            return false;
        value = _value;
        return true;
    }
};
