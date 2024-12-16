using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Principal;

namespace program.Domain;

public class User
{
    public int Id { get; set; }

    [EmailAddress]
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(128)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public static User? FromClaimsPrincipal(ClaimsPrincipal cp){
        if (cp.Identity!.IsAuthenticated is false)
            return null;
        return new() {
            Name = cp.Identity!.Name!,
            Email = cp.FindFirstValue(ClaimTypes.Email)!
        };
    }
}