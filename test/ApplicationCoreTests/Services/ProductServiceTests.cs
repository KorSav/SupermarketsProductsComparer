using ApplicationCore.DTOs;
using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;
using ApplicationCore.Services;
using NSubstitute;

namespace ApplicationCore.UnitTests.Services;

public sealed class ProductServiceTests
{
    [Fact]
    public async Task GetProductsAsync_CapsRepositoryPageLimitAndReturnsPaginatedProducts()
    {
        var request = new Request("milk", SortBy.Price, SortOrder.Asc);
        var products = new[] { Product("Milk", 10), Product("Cheese", 20) };
        var productRepo = Substitute.For<IProductRepository>();
        var requestRepo = Substitute.For<IRequestRepository>();
        productRepo
            .FindPageByQueryAsync(Arg.Any<ProductPageQueryDto>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new PageResultDto<Product>(products, 15)));
        var service = new ProductService(productRepo, requestRepo);

        var result = await service.GetProductsAsync(
            request,
            page: 2,
            pageLimit: 100,
            CancellationToken.None
        );

        Assert.Equal(2, result.Count);
        Assert.Equal(15, result.TotalItems);
        Assert.Equal(2, result.PageNo);
        Assert.Equal(30, result.PageSize);
        await productRepo
            .Received(1)
            .FindPageByQueryAsync(
                Arg.Is<ProductPageQueryDto>(q =>
                    q.Skip == 60 && q.Take == 30 && ReferenceEquals(q.Request, request)
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task AuthnGetProductsAsync_ReturnsProductsAndTrueStoredFlagWhenRequestIsSaved()
    {
        var request = new Request("apple", SortBy.Name, SortOrder.Desc);
        var userId = Guid.NewGuid();
        var products = new[] { Product("Apple", 12) };
        var storedRequests = new List<StoredRequest>
        {
            new(Guid.NewGuid(), userId, new Request("banana", SortBy.Name, SortOrder.Asc)),
            new(Guid.NewGuid(), userId, request),
        };
        var productRepo = Substitute.For<IProductRepository>();
        var requestRepo = Substitute.For<IRequestRepository>();
        productRepo
            .FindPageByQueryAsync(Arg.Any<ProductPageQueryDto>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new PageResultDto<Product>(products, 1)));
        requestRepo
            .FindAllByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(storedRequests));
        var service = new ProductService(productRepo, requestRepo);

        var result = await service.AuthnGetProductsAsync(
            request,
            page: 0,
            pageLimit: 10,
            userId,
            CancellationToken.None
        );

        Assert.True(result.IsStored);
        Assert.Single(result.Products);
        Assert.Equal(products[0], result.Products[0]);
        await requestRepo.Received(1).FindAllByUserIdAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AuthnGetProductsAsync_ReturnsFalseStoredFlagWhenRequestIsNotSaved()
    {
        var request = new Request("apple", SortBy.Name, SortOrder.Desc);
        var userId = Guid.NewGuid();
        var productRepo = Substitute.For<IProductRepository>();
        var requestRepo = Substitute.For<IRequestRepository>();
        productRepo
            .FindPageByQueryAsync(Arg.Any<ProductPageQueryDto>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new PageResultDto<Product>([], 0)));
        requestRepo
            .FindAllByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    new List<StoredRequest>
                    {
                        new(
                            Guid.NewGuid(),
                            userId,
                            new Request("banana", SortBy.Name, SortOrder.Asc)
                        ),
                    }
                )
            );
        var service = new ProductService(productRepo, requestRepo);

        var result = await service.AuthnGetProductsAsync(
            request,
            page: 0,
            pageLimit: 10,
            userId,
            CancellationToken.None
        );

        Assert.False(result.IsStored);
        Assert.Empty(result.Products);
    }

    private static Product Product(string name, decimal price) =>
        new(
            Guid.NewGuid(),
            name,
            string.Empty,
            price,
            new Measure(1, MeasureUnit.Count),
            new Uri("https://example.com/product"),
            new Uri("https://example.com/image"),
            Shop.Fozzy
        );
}
