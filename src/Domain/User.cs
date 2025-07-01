using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PriceComparer.Domain;

public class User
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = null!;

    [MaxLength(128)]
    public string PasswordHash { get; set; } = null!;

    public static User? FromClaimsPrincipal(ClaimsPrincipal cp)
    {
        if (cp.Identity!.IsAuthenticated is false)
            return null;
        return new()
        {
            Id = int.Parse(cp.FindFirst("Id")!.Value),
            Name = cp.Identity!.Name!,
            Email = cp.FindFirst(ClaimTypes.Email)!.Value,
        };
    }
}
