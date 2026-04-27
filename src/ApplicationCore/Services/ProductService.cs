using ApplicationCore.DTOs;
using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;
using ApplicationCore.Exceptions;
using ApplicationCore.Utils;

namespace ApplicationCore.Services;

public class ProductService(IProductRepository productRepo, IRequestRepository requestRepo)
{
    private const int _pageLimit = 30;

    public async Task<PaginatedList<Product>> GetProductsAsync(Request request, int page)
    {
        ProductPageQueryDto pageQuery = new(page, _pageLimit, request);
        var pageResult = await productRepo.FindPageByQueryAsync(pageQuery, CancellationToken.None);
        return new PaginatedList<Product>(pageResult.Items, pageResult.Total, page, _pageLimit);
    }

    public async Task<PaginatedList<Product>> GetProductsAsync(
        Guid storedRequestId,
        Guid userId,
        int page
    )
    {
        var allStored = await requestRepo.FindAllByUserIdAsync(userId, CancellationToken.None);
        StoredRequest request =
            allStored.FirstOrDefault(r => r.Id == storedRequestId)
            ?? throw NotFoundExceptionType.StoredRequestDoesNotExist.New();

        return await GetProductsAsync(request, page);
    }
}
