using System.Globalization;
using ApplicationCore;
using ApplicationCore.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repository;

internal class BulkUpsertScope : IBulkUpsertScope
{
    internal const string TempTableName = "bulk_insert_table";
    private bool _terminated = false;
    private readonly AppDbContext _dbContext;

    private BulkUpsertScope(AppDbContext dbContext) => _dbContext = dbContext;

    public static async Task<BulkUpsertScope> CreateNewAsync(
        AppDbContext dbContext,
        CancellationToken ct
    )
    {
        await dbContext.Database.OpenConnectionAsync(ct);
        await dbContext.Database.ExecuteSqlRawAsync(
            $"""
            CREATE TEMP TABLE {TempTableName} (
                shop text,
                name text,
                price numeric(8,2),
                unified_price numeric(8,2),
                amount numeric(10,4),
                unit text,
                full_link_product text,
                full_link_image text
            )
            """,
            ct
        );
        return new(dbContext);
    }

    public async Task UpsertAsync(IReadOnlyCollection<Product> bulk, CancellationToken ct)
    {
        ThrowIfTerminated();
        // copied into table in csv format (https://www.postgresql.org/docs/current/sql-copy.html#SQL-COPY-FILE-FORMATS:~:text=not%20all%20alike.-,CSV%20Format,-This%20format%20option)
        var conn = (NpgsqlConnection)_dbContext.Database.GetDbConnection();
        using var writer = await conn.BeginTextImportAsync(
            $"""
            COPY {TempTableName} FROM STDIN
            WITH (FORMAT csv)
            """,
            ct //npgsql uses utf-8 encoding
        );
        foreach (var p in bulk)
        {
            var unifiedPrice = p.WithUnifiedPrice().Price;
            await writer.WriteLineAsync(p.ToStringCSV(unifiedPrice));
        }
    }

    public async Task CommitAsync(CancellationToken ct)
    {
        ThrowIfTerminated();
        await _dbContext.Database.ExecuteSqlRawAsync(
            $"""
            INSERT INTO "Products"("Shop", "Name", "Price", "UnifiedPrice", "Amount", "Unit", "FullLinkProduct", "FullLinkImage")
            SELECT * FROM {TempTableName}
            """,
            ct
        );
        await _dbContext.Database.CloseConnectionAsync();
        _terminated = true;
    }

    public async Task RollbackAsync()
    {
        ThrowIfTerminated();
        // db will drop temp table when connection is in any case closed
        await _dbContext.Database.CloseConnectionAsync();
        _terminated = true;
    }

    private void ThrowIfTerminated()
    {
        if (_terminated)
            throw new InvalidOperationException(
                $"Don't use the {nameof(IBulkUpsertScope)} after calling either {nameof(CommitAsync)} or {nameof(RollbackAsync)}"
            );
    }
}

// better file, but UTs don't see such
internal static class Extensions
{
    public static string GenericToStringCSV<T>(this T obj)
    {
        if (obj is null)
            return "";
        var str = obj switch
        {
            DateTime o => o.ToString("O", CultureInfo.InvariantCulture),
            DateTimeOffset o => o.ToString("O", CultureInfo.InvariantCulture),
            IFormattable o => o.ToString(null, CultureInfo.InvariantCulture),
            _ => obj.ToString(),
        };
        return str.ToStringCSV();
    }

    public static string ToStringCSV(this string? str)
    {
        if (str is null)
            return "";
        return $"\"{str.Replace("\"", "\"\"")}\"";
    }

    public static string ToStringCSV(this Product p, decimal unifiedPrice) =>
        string.Join(
            ',',
            p.Shop.GenericToStringCSV(),
            p.Name.ToStringCSV(),
            p.Price.GenericToStringCSV(),
            unifiedPrice.GenericToStringCSV(),
            p.Measure.Count.GenericToStringCSV(),
            p.Measure.Unit.GenericToStringCSV(),
            p.LinkProduct.GenericToStringCSV(),
            p.LinkImage.GenericToStringCSV()
        );
}
