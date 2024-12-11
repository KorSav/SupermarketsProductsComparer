using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace program.Domain;

public class Request
{
    [Key]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [Key]
    public string Name { get; set; } = string.Empty;

    public User User { get; set; } = new();
}