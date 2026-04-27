using System.ComponentModel.DataAnnotations;
using ApplicationCore.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Entities;

[Keyless]
internal class ScrapedProductStage
{
    public Shop Shop { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [Precision(14, 2)]
    public decimal Price { get; set; }

    [Precision(14, 2)]
    public decimal UnifiedPrice { get; set; }

    [Precision(14, 2)]
    public decimal Amount { get; set; }

    public MeasureUnit Unit { get; set; }

    [MaxLength(2048)]
    public string FullLinkProduct { get; set; } = null!;

    [MaxLength(2048)]
    public string FullLinkImage { get; set; } = null!;
}
