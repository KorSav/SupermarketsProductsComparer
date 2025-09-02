using PriceComparer.Application.Products.Types;

namespace PriceComparer.Application.Common;

public record SortOptions(SortBy SortBy = SortBy.Name, SortOrder Order = SortOrder.Asc);
