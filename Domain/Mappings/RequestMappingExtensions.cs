using program.Models.Request;

namespace program.Domain.Mappings;

public static class RequestMappingExtensions
{
    public static Request ToRequest(this RequestViewModel requestViewModel, int userId) =>
        new()
        {
            UserId = userId,
            Name = requestViewModel.Find,
            Sort = requestViewModel.SortBy,
            SortOrder = requestViewModel.SortOrder,
        };

    public static RequestInfoViewModel ToRequestViewModel(this Request request) =>
        new(Find: request.Name, SortBy: request.Sort, SortOrder: request.SortOrder);
}
