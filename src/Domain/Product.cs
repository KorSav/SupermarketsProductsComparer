using System.ComponentModel.DataAnnotations;

// using Microsoft.EntityFrameworkCore;

namespace PriceComparer.Domain;

public class Product
{
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    // [Precision(14, 2)]
    public decimal PriceUnified { get; set; }

    // [Precision(14, 2)]
    public decimal PriceInitial { get; set; }

    [Key]
    [MaxLength(2048)]
    public string FullLinkProduct { get; set; } = null!;

    [MaxLength(2048)]
    public string FullLinkImage { get; set; } = null!;

    public Measure Measure { get; set; }

    public ProductStatus ProductStatus { get; set; }

    public Shop Shop { get; set; }
}
