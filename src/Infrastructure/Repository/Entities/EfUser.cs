using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Entities;

[Index(nameof(Name), nameof(Surname), IsUnique = true)]
public class EfUser
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public EfUser() { }

    public EfUser(string name, string surname, string passwordHash)
    {
        Name = name;
        Surname = surname;
        PasswordHash = passwordHash;
    }

    public User ToUser() => new(Id, Name, Surname);
}
