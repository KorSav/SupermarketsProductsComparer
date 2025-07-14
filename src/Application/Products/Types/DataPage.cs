namespace PriceComparer.Application.Products.Types;

public record DataPage(int No, int Size)
{
    public DataWindow ToWindow() => new(Skip: (No - 1) * Size, Take: Size);
};
