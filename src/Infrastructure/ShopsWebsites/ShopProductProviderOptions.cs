using System.ComponentModel.DataAnnotations;

namespace Infrastructure.ShopsWebsites;

public class ShopProductProviderOptions
{
    [Required]
    [Range(typeof(TimeSpan), "00:00:03", "00:01:00")]
    public TimeSpan DelayBetweenRequests { get; set; }
}
