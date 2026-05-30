using ApplicationCore.Entities.Request;
using ApplicationCore.Exceptions;
using ApplicationCore.Services;
using NSubstitute;

namespace ApplicationCore.UnitTests.Services;

public sealed class StoredRequestsServiceTests
{
    [Fact]
    public async Task GetAllForUserAsync_ReturnsRequestsFromRepository()
    {
        var userId = Guid.NewGuid();
        var stored = new List<StoredRequest>
        {
            new(Guid.NewGuid(), userId, Request("milk")),
            new(Guid.NewGuid(), userId, Request("bread")),
        };
        var repo = Substitute.For<IRequestRepository>();
        repo.FindAllByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(stored));
        var service = new StoredRequestsService(repo);

        var result = await service.GetAllForUserAsync(userId, CancellationToken.None);

        Assert.Same(stored, result);
        await repo.Received(1).FindAllByUserIdAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ToggleAsync_AddsRequestWhenMissingAndLimitIsNotReached()
    {
        var userId = Guid.NewGuid();
        var request = Request("milk");
        var repo = Substitute.For<IRequestRepository>();
        repo.FindAllByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<StoredRequest>()));
        repo.AddNewAsync(
                request,
                userId,
                StoredRequestsService.MaxLimit,
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult(new StoredRequest(Guid.NewGuid(), userId, request)));
        var service = new StoredRequestsService(repo);

        await service.ToggleAsync(request, userId, CancellationToken.None);

        await repo.Received(1)
            .AddNewAsync(
                request,
                userId,
                StoredRequestsService.MaxLimit,
                Arg.Any<CancellationToken>()
            );
        await repo.DidNotReceiveWithAnyArgs()
            .RemoveByIdAsync(default, default, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ToggleAsync_RemovesRequestWhenSameRequestAlreadyExistsForCurrentUser()
    {
        var userId = Guid.NewGuid();
        var request = Request("milk");
        var stored = new StoredRequest(Guid.NewGuid(), userId, request);
        var repo = Substitute.For<IRequestRepository>();
        repo.FindAllByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<StoredRequest> { stored }));
        var service = new StoredRequestsService(repo);

        await service.ToggleAsync(request, userId, CancellationToken.None);

        await repo.Received(1).RemoveByIdAsync(stored.Id, userId, Arg.Any<CancellationToken>());
        await repo.DidNotReceiveWithAnyArgs()
            .AddNewAsync(default!, default, default, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ToggleAsync_ThrowsWhenAddingRequestWouldExceedLimit()
    {
        var userId = Guid.NewGuid();
        var request = Request("new request");
        var stored = Enumerable
            .Range(0, StoredRequestsService.MaxLimit)
            .Select(i => new StoredRequest(Guid.NewGuid(), userId, Request($"stored {i}")))
            .ToList();
        var repo = Substitute.For<IRequestRepository>();
        repo.FindAllByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(stored));
        var service = new StoredRequestsService(repo);

        var exception = await Assert.ThrowsAsync<DomainException<ValidationExceptionType>>(
            () => service.ToggleAsync(request, userId, CancellationToken.None)
        );

        Assert.Equal(ValidationExceptionType.StoredRequestsLimitReached, exception.Type);
        await repo.DidNotReceiveWithAnyArgs()
            .AddNewAsync(default!, default, default, TestContext.Current.CancellationToken);
        await repo.DidNotReceiveWithAnyArgs()
            .RemoveByIdAsync(default, default, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ToggleAsync_ThrowsWhenStoredRequestBelongsToDifferentUser()
    {
        var currentUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var request = Request("milk");
        var stored = new StoredRequest(Guid.NewGuid(), otherUserId, request);
        var repo = Substitute.For<IRequestRepository>();
        repo.FindAllByUserIdAsync(currentUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<StoredRequest> { stored }));
        var service = new StoredRequestsService(repo);

        var exception = await Assert.ThrowsAsync<DomainException<ValidationExceptionType>>(
            () => service.ToggleAsync(request, currentUserId, CancellationToken.None)
        );

        Assert.Equal(ValidationExceptionType.NotAllowed, exception.Type);
        await repo.DidNotReceiveWithAnyArgs()
            .AddNewAsync(default!, default, default, TestContext.Current.CancellationToken);
        await repo.DidNotReceiveWithAnyArgs()
            .RemoveByIdAsync(default, default, TestContext.Current.CancellationToken);
    }

    private static Request Request(string searchString) =>
        new(searchString, SortBy.Name, SortOrder.Asc);
}
