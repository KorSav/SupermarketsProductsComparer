using System.ComponentModel.DataAnnotations;
using program.Domain.Enums;

namespace program.Domain;

public class Shop
{
    public ShopId Id { get; set; }
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    public List<Product> Products {get; set; } = [];
}