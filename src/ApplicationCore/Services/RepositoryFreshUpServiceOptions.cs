using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Services;

public class RepositoryFreshUpServiceOptions
{
    [Required]
    [Range(typeof(TimeSpan), "01:00:00", "24:00:00")]
    public TimeSpan Interval { get; set; }
}
