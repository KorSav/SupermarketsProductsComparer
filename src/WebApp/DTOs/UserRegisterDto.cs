using System.ComponentModel.DataAnnotations;
using FoolProof.Core;

namespace WebApp.DTOs;

public class UserRegisterDto
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [EqualTo(nameof(Password))]
    [Required]
    public string PasswordDup { get; set; } = null!;
}
