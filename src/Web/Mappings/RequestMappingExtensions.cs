using PriceComparer.Domain;
using PriceComparer.Web.Models.Request;

namespace PriceComparer.Web.Mappings;

public static class RequestMappingExtensions
{
    public static Request ToRequest(this RequestViewModel requestViewModel, int userId) =>
        new()
        {
            User = new(), // FIXME: stub
            Name = requestViewModel.Find,
            Sort = requestViewModel.SortBy,
            SortOrder = requestViewModel.SortOrder,
        };

    public static RequestInfoViewModel ToRequestViewModel(this Request request) =>
        new(Find: request.Name, SortBy: request.Sort, SortOrder: request.SortOrder);
}
