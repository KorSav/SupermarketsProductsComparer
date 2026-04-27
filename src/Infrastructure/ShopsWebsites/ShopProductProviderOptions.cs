using System.ComponentModel.DataAnnotations;

namespace Infrastructure.ShopsWebsites;

public class ShopProductProviderOptions
{
    [Required]
    public TimeSpan DelayBetweenRequests { get; set; }
}
