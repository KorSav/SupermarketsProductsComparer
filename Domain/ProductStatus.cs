using System.ComponentModel.DataAnnotations;
using program.Domain.Enums;

namespace program.Domain;

public class ProductStatus
{
    public ProductStatusId Id { get; set; }

    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = [];
}
