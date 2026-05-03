using static System.Reflection.BindingFlags;

namespace ApplicationCore.Utils;

public record TypedError<T> : Error
{
    public TypedError(string typeMember, string description)
        : base(typeMember, description)
    {
        var matchingMember =
            _allowedErrorFields.FirstOrDefault(f =>
                f.Equals(typeMember, StringComparison.OrdinalIgnoreCase)
            )
            ?? throw new ArgumentException(
                $"Error`1.{nameof(Reason)} could contain only name of public field or property of {typeof(T).FullName}. Got unknown member '{typeMember}'",
                nameof(typeMember)
            );
        Reason = matchingMember;
        Description = description;
    }

    private static readonly string[] _allowedErrorFields;

    static TypedError()
    {
        Type type = typeof(T);
        var bindingFlags = Public | Instance;
        var fieldsNames = type.GetFields(bindingFlags).Select(fi => fi.Name);
        var propsNames = type.GetProperties(bindingFlags)
            .Where(pi => pi.CanRead)
            .Select(pi => pi.Name);
        _allowedErrorFields = [.. fieldsNames, .. propsNames];
    }
}

public record Error(string Reason, string Description) { }
