using System.ComponentModel.DataAnnotations;

namespace Infrastructure.ShopsWebsites.Fozzy;

public class FozzyDataRetrieverOptions : ShopDataRetrieverOptions
{
    [Required]
    public TimeSpan PaginationDelay { get; set; }

    public FozzyDataRetrieverOptions(TimeSpan paginationDelay, ShopDataRetrieverOptions other)
        : base(other)
    {
        PaginationDelay = paginationDelay;
    }

    /// <summary>
    /// Used by options pattern binder
    /// </summary>
    public FozzyDataRetrieverOptions()
        : base() { }
}
