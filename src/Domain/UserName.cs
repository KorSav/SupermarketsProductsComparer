using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PriceComparer.Domain;

public readonly partial struct UserName(string name) : IEquatable<UserName>
{
    [GeneratedRegex(@"^([a-zA-Z]+|\p{IsCyrillic}+)$")]
    public static partial Regex LatinOrCyrrilicRegex();

    public static readonly int MaxLength = 50;
    public static readonly int MinLength = 5;
    public static readonly bool IsNullOrWhiteSpace = false;
    public static readonly bool IsCaseSensitive = false;

    static string Validate(string name)
    {
        name = name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name should not be null or empty");

        StringInfo si = new(name);
        if (si.LengthInTextElements < MinLength)
            throw new ArgumentException(
                $"User name could not be shorter than {MinLength} characters"
            );
        if (si.LengthInTextElements > MaxLength)
            throw new ArgumentException(
                $"User name could not be longer than {MaxLength} characters"
            );

        if (!LatinOrCyrrilicRegex().IsMatch(name))
            throw new ArgumentException($"User name should match regex: {LatinOrCyrrilicRegex()}");

        return name;
    }

    readonly string? _value = Validate(name); // null if created using 'default'
    public readonly string Value
    {
        get =>
            _value is not null
                ? _value
                : throw new InvalidOperationException(
                    $"Cannot instantiate struct {typeof(UserName).FullName} using default value"
                );
    }

    public readonly string Normalized
    {
        get =>
            _value is null
                ? throw new InvalidOperationException(
                    $"Cannot instantiate struct {typeof(UserName).FullName} using default value"
                )
                : _value.ToLowerInvariant();
    }

    public readonly bool Equals(UserName other) =>
        StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public override readonly int GetHashCode() =>
        StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public override readonly bool Equals([NotNullWhen(true)] object? obj) =>
        obj is UserName vn && vn.Equals(this);

    public static bool operator ==(UserName left, UserName right) => left.Equals(right);

    public static bool operator !=(UserName left, UserName right) => !(left == right);
}
