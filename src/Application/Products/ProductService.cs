using PriceComparer.Application.Common;
using PriceComparer.Application.Products.DTOs;
using PriceComparer.Application.Products.Provider;
using PriceComparer.Application.Products.Types;

namespace PriceComparer.Application.Products;

public class ProductService(IProductRepository productRepo) : IProductService
{
    readonly IProductRepository _repo = productRepo;

    public async Task<PaginatedList<ProductInfoDto>> GetAllProducts(
        DataPage dataPage,
        SortOptions sortOptions,
        CancellationToken cancellationToken
    )
    {
        var (products, total) = await _repo.GetAllAsync(
            dataPage.ToWindow(),
            sortOptions,
            cancellationToken
        );
        var pinfos = products.Select(sp => sp.ProductDto).ToList();
        return new(pinfos, total, dataPage);
    }

    public async Task<PaginatedList<ProductInfoDto>> FindProductsByNameAsync(
        string prodName,
        DataPage page,
        SortOptions sortOptions,
        CancellationToken cancellationToken
    )
    {
        var (products, total) = await _repo.FuzzyFindByNameAsync(
            prodName,
            page.ToWindow(),
            sortOptions,
            cancellationToken
        );
        var pinfos = products.Select(sp => sp.ProductDto).ToList();
        return new(pinfos, total, page);
    }

    /// <summary>
    /// Adds data into product repo infinitely with given time interval until cancelled
    /// </summary>
    public async Task UpdateRepoPeriodicallyUntilCancelAsync(
        IProductProvider provider,
        TimeSpan interval,
        CancellationToken cancellationToken
    )
    {
        try
        {
            PeriodicTimer timer = new(interval);
            do
            {
                await _repo.UpdateAllStatusAsync(ProductStatus.NeedRemoval);
                var providedProdInfos = await provider.GetProductsAsync(cancellationToken);
                var unifiedProducts = providedProdInfos
                    .Select(pinfo => new StoredProductDto(
                        pinfo,
                        pinfo.ProductPrice.ToUnified(),
                        ProductStatus.Updated
                    ))
                    .ToList();
                await _repo.CreateOrUpdateAsync(unifiedProducts);
                await _repo.DeleteAllWithStatusAsync(ProductStatus.NeedRemoval);
            } while (await timer.WaitForNextTickAsync(cancellationToken));
        }
        catch (OperationCanceledException)
        {
            await _repo.UpdateAllStatusAsync(ProductStatus.Updated);
            throw;
        }
    }
}
