using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PriceComparer.Domain.Enums;

namespace PriceComparer.Domain;

public class Request
{
    public User User { get; set; } = null!;

    [MaxLength(200)]
    public string Name { get; set; } = null!;

    public SortBy Sort { get; set; }

    public SortOrder SortOrder { get; set; }
}
