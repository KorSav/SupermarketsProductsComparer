using ApplicationCore.Entities.Product;

namespace ApplicationCore;

public interface IBulkUpsertScope
{
    /// <summary>
    /// Makes sure that none of the changes done during refresh will be visible
    /// </summary>
    public Task RollbackAsync();
    Task UpsertAsync(IReadOnlyCollection<Product> bulk, CancellationToken cancellationToken);

    /// <summary>
    /// Commits all of the changes being done during refresh
    /// </summary>
    public Task CommitAsync(CancellationToken cancellationToken);
}
