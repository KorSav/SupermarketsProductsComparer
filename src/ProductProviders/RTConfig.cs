namespace PriceComparer.ProductProvider;

internal record RTConfig(
    TimeSpan DelayBetweenRequests,
    int MaxProdCountPerRequest,
    TimeSpan PaginationDelay
);
