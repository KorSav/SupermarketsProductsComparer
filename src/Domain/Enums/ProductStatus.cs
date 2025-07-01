namespace PriceComparer.Domain.Enums;

// Do not change underlying values => one to one db ID mapping
public enum ProductStatus : byte
{
    Updated = 0,
    NeedRemoval = 1,
}
