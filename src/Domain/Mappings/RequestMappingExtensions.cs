using program.Models.Request;

namespace program.Domain.Mappings;

public static class RequestMappingExtensions{
    public static Request ToRequest(this RequestViewModel requestViewModel, int userId) =>
        new(){
            UserId = userId,
            Name = requestViewModel.Find,
            SortId = requestViewModel.SortBy,
            SortOrderId = requestViewModel.SortOrder
        };
    public static RequestInfoViewModel ToRequestViewModel(this Request request) =>
        new(
            Find: request.Name,
            SortBy: request.SortId,
            SortOrder: request.SortOrderId
        );
}