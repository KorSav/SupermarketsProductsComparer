using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using program.Domain.Enums;

namespace program.Domain;

public class Product
{
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

    public Measure Measure { get; set; }

    public ProductStatus ProductStatus { get; set; }

    public Shop Shop { get; set; }
}
