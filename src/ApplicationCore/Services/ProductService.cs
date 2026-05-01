using ApplicationCore.DTOs;
using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;
using ApplicationCore.Utils;

namespace ApplicationCore.Services;

public class ProductService(IProductRepository productRepo)
{
    private const int _pageLimit = 30;

    public async Task<PaginatedList<Product>> GetProductsAsync(Request request, int page)
    {
        ProductPageQueryDto pageQuery = new(page, _pageLimit, request);
        var pageResult = await productRepo.FindPageByQueryAsync(pageQuery, CancellationToken.None);
        return new PaginatedList<Product>(pageResult.Items, pageResult.Total, page, _pageLimit);
    }
}
