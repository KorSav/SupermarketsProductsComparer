using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using program.Domain.Enums;

namespace program.Domain;

public class Product
{
    [Key]
    [ForeignKey(nameof(Shop))]
    public ShopId ShopId { get; set; }

    [Key]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Precision(14, 2)]
    public decimal Price { get; set; }

    [MaxLength(100)]
    public string FullLinkProduct { get; set; } = string.Empty;

    [MaxLength(100)]
    public string FullLinkImage { get; set; } = string.Empty;

    [Required]
    public MeasureId MeasureId { get; set; }
    public Measure Measure { get; set; } = new();

    [Required]
    public ProductStatusId ProductStatusId { get; set; }
    public ProductStatus ProductStatus { get; set; } = new();

    public Shop Shop { get; set; } = new();
}
