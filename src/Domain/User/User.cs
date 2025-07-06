namespace PriceComparer.Domain;

public partial class User(int id, User.ValidName name, User.IEmail? email = null)
{
    public int Id { get; } = id;
    public ValidName Name { get; set; } = name;

    public interface IEmail : IEquatable<IEmail> { }

    public IEmail? Email { get; set; } = email;

    public (bool IsConflict, string? SamePropertyName) ConflictsWith(User other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (Id.Equals(other.Id))
            return (true, nameof(Id));

        if (Email is null || other.Email is null)
            return (false, null);

        if (Email.Equals(other.Email))
            return (true, nameof(Email));

        return (false, null);
    }
}
