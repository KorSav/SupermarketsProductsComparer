using System.ComponentModel.DataAnnotations;
using program.Domain.Enums;

namespace program.Domain.EnumClasses;

public class Shop : IEnumClass<ShopId>
{
    public ShopId Id { get; set; }
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = [];
}