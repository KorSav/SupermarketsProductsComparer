using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using program.Controllers.Enums;
using program.Domain.EnumClasses;
using program.Domain.Enums;

namespace program.Domain;

public class Request
{
    [Key]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [Key]
    public string Name { get; set; } = string.Empty;

    public SortBy SortId { get; set; }
    public Sort Sort { get; set; } = null!;

    public SortOrderId SortOrderId { get; set; }
    public SortOrder SortOrder { get; set; } = null!;

    public User User { get; set; } = null!;
}