using System.ComponentModel.DataAnnotations;
using FoolProof.Core;

namespace PriceComparer.Web.Models.User;

public class UserLogInViewModel
{
    [RequiredIfEmpty(nameof(Email))]
    public string Name { get; set; } = string.Empty;

    [RequiredIfEmpty(nameof(Name))]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}