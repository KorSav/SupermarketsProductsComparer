namespace PriceComparer.Application.Products.Types;

// Change underlying values to keep backward compatibility
public enum ProductStatus : byte
{
    Updated = 0,
    NeedRemoval = 1,
}
