using ApplicationCore.DTOs;
using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;
using ApplicationCore.Utils;

namespace ApplicationCore.Services;

public class ProductService(IProductRepository productRepo, IRequestRepository requestRepo)
{
    private const int _maxPageLimit = 30;

    public async Task<PaginatedList<Product>> GetProductsAsync(
        Request request,
        int page,
        int pageLimit,
        CancellationToken ct
    )
    {
        var limit = int.Min(_maxPageLimit, pageLimit);
        ProductPageQueryDto pageQuery = new(page, limit, request);
        var pageResult = await productRepo.FindPageByQueryAsync(pageQuery, ct);
        return new PaginatedList<Product>(pageResult.Items, pageResult.Total, page, _maxPageLimit);
    }

    public async Task<AuthnGetProductsResult> AuthnGetProductsAsync(
        Request request,
        int page,
        int pageLimit,
        Guid userId,
        CancellationToken ct
    )
    {
        var allStored = await requestRepo.FindAllByUserIdAsync(userId, ct);
        bool isStored = allStored.Any(e => e.Request == request);
        var products = await GetProductsAsync(request, page, pageLimit, ct);
        return new(products, isStored);
    }
}

public record AuthnGetProductsResult(PaginatedList<Product> Products, bool IsStored);
