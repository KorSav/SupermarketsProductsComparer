using System.ComponentModel.DataAnnotations;

namespace program.Domain;

public class User
{
    public int Id { get; set; }

    [EmailAddress]
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}