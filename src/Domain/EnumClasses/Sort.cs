using System.ComponentModel.DataAnnotations;
using program.Domain.Enums;

namespace program.Domain.EnumClasses;

public class Sort : IEnumClass<SortBy>
{
    public SortBy Id { get; set; }

    [MaxLength(30)]
    public string Name { get; set; } = null!;
}