using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ApplicationCore.Entities.Request;

namespace WebApp.DTOs;

public record ToggleRequestDto(
    [Required(AllowEmptyStrings = false)] string Find,
    [property: JsonRequired] SortOrder SortOrder,
    [property: JsonRequired] SortBy SortBy
)
{
    public Request ToRequest() => new(Find, SortBy, SortOrder);
}
