using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Entities;

[Index(nameof(Name), nameof(Email), IsUnique = true)]
public class EfUser
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public EfProductList? ProductList { get; set; }

    public ICollection<EfPurchase> Purchases { get; set; } = [];

    public EfUser() { }

    public EfUser(string name, string email, string passwordHash)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }

    public User ToUser() => new(Id, Name, Email);
}
