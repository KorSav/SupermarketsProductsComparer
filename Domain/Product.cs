using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using program.Domain.EnumClasses;
using program.Domain.Enums;

namespace program.Domain;

public class Product
{
    [ForeignKey(nameof(Shop))]
    public ShopId ShopId { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Precision(14, 2)]
    public decimal PriceUnified { get; set; }

    [Required]
    [Precision(14, 2)]
    public decimal PriceInitial { get; set; }

    [Key]
    [MaxLength(2048)]
    public string FullLinkProduct { get; set; } = string.Empty;

    [MaxLength(2048)]
    public string FullLinkImage { get; set; } = string.Empty;

    [Required]
    public MeasureId MeasureId { get; set; }
    public Measure Measure { get; set; } = null!;

    [Required]
    public ProductStatusId ProductStatusId { get; set; }
    public ProductStatus ProductStatus { get; set; } = null!;

    public Shop Shop { get; set; } = null!;
}
