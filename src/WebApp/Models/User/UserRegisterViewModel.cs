using System.ComponentModel.DataAnnotations;
using FoolProof.Core;

namespace program.Models.User;

public class UserRegisterViewModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [EqualTo(nameof(Password))]
    [Required]
    public string PasswordDup { get; set; } = null!;
}