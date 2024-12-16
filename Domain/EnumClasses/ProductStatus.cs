using System.ComponentModel.DataAnnotations;
using program.Domain.Enums;

namespace program.Domain.EnumClasses;

public class ProductStatus : IEnumClass<ProductStatusId>
{
    public ProductStatusId Id { get; set; }

    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = null!;
}
