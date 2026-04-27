using ApplicationCore.DTOs;
using ApplicationCore.Entities.Product;

namespace ApplicationCore;

public interface IProductRepository
{
    Task<PageResultDto<Product>> FindPageByQueryAsync(
        ProductPageQueryDto pageQueryDto,
        CancellationToken cancellationToken
    );
    Task<IBulkUpsertScope> BeginBulkUpsertAsync(CancellationToken cancellationToken);
}
