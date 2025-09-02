namespace PriceComparer.Application.Products.Types;

// Do not change underlying values => one to one db ID mapping
public enum SortBy : byte
{
    Name = 0,
    UnifiedPrice = 1,
    Price = 2,
}
