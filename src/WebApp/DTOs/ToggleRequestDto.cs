using System.ComponentModel.DataAnnotations;
using ApplicationCore.Entities.Request;

namespace WebApp.DTOs;

public record ToggleRequestDto(
    [Required(AllowEmptyStrings = false)] string Find,
    [Required] SortOrder SortOrder,
    [Required] SortBy SortBy
)
{
    public Request ToRequest() => new(Find, SortBy, SortOrder);
}
