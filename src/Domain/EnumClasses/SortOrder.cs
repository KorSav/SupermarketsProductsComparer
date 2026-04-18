using System.ComponentModel.DataAnnotations;
using program.Controllers.Enums;

namespace program.Domain.EnumClasses;

public class SortOrder : IEnumClass<SortOrderId>
{
    public SortOrderId Id { get; set; }

    [MaxLength(5)]
    public string Name { get; set; } = null!;
}