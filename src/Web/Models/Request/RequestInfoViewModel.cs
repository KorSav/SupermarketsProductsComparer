using PriceComparer.Domain.Enums;

namespace PriceComparer.Web.Models.Request;

public record RequestInfoViewModel(string Find, SortBy SortBy, SortOrder SortOrder);
