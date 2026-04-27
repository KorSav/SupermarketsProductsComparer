using System.ComponentModel.DataAnnotations;

namespace Infrastructure.ShopsWebsites.Fozzy;

public class FozzyDataRetrieverOptions(TimeSpan paginationDelay, ShopDataRetrieverOptions other)
    : ShopDataRetrieverOptions(other)
{
    [Required]
    public TimeSpan PaginationDelay { get; set; } = paginationDelay;
}
