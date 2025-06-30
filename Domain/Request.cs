using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using program.Domain.Enums;

namespace program.Domain;

public class Request
{
    [Key]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [Key]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public SortBy Sort { get; set; }

    public SortOrder SortOrder { get; set; }

    public User User { get; set; } = null!;
}
