using System.Data;
using ApplicationCore;
using ApplicationCore.Entities;
using ApplicationCore.Entities.Request;
using ApplicationCore.Exceptions;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static ApplicationCore.Exceptions.ConflictExceptionType;
using static ApplicationCore.Exceptions.ValidationExceptionType;

namespace InfrastructureIntegrationTests.RepositoryTests;

[Collection(Collections.Container1)]
public class RequestRepositoryTests(DbContainerFixture _) : DbPerTestCaseBase(_)
{
    [Fact]
    public async Task AddAsync_AddsNewItemInDb_WhenLimitIsNotReached()
    {
        // Arrange
        User user = await CreateUserAsync();
        // Act
        StoredRequest storedRequest = await CreateRequestForAsync("some request", user);
        // Assert
        await using (var scope = Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var act = await dbContext
                .Requests.Where(e => e.Id == storedRequest.Id)
                .Select(e => e.ToStoredRequest())
                .ToListAsync(CancellationToken);
            Assert.Equal([storedRequest], act);
        }
    }

    [Fact]
    public async Task AddAsync_ThrowsValidationException_WhenRequestsLimitExceeded()
    {
        // Arrange
        User user = await CreateUserAsync();
        await CreateRequestForAsync("some request", user);

        // Act
        Barrier barrier = new(3);
        async Task SaveNewRequest(int i)
        {
            await using var scope = Services.CreateAsyncScope();
            var repo =
                scope.ServiceProvider.GetRequiredService<IRequestRepository>() as RequestRepository;
            Assert.NotNull(repo);
            repo.DelayBeforeCheckingAmount = TimeSpan.FromMilliseconds(500);
            var sortOrder = (SortOrder)(i % 2);
            var sortBy = (SortBy)(i / 2 % 3);
            Request newRequest = new($"search {i}", sortBy, sortOrder);
            barrier.SignalAndWait();
            await repo.AddNewAsync(newRequest, user.Id, maxCount: 2, CancellationToken);
        }
        Task all = await WhenAllAsync(SaveNewRequest, SaveNewRequest, SaveNewRequest);

        // Assert
        if (all.Exception is not null)
            Assert.All(
                all.Exception.InnerExceptions,
                ex =>
                {
                    Assert.True(
                        ex
                            is DomainException<ValidationExceptionType>
                            {
                                Type: StoredRequestsLimitReached or TooManyRequest,
                            },
                        $"Expected exception to be {typeof(DomainException<ValidationExceptionType>)} with type either "
                            + $"{StoredRequestsLimitReached} or {TooManyRequest}. "
                            + $"But got {ex}. "
                    );
                }
            );
        await using (var scope = Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var act = await dbContext
                .Requests.Where(e => e.UserId == user.Id)
                .CountAsync(CancellationToken);
            Assert.Equal(2, act);
        }
    }

    [Fact]
    public async Task RemoveById_DoesNotThrow_WhenDeletingSameItem()
    {
        // Arrange
        var user = await CreateUserAsync();
        var storedRequest = await CreateRequestForAsync("some request", user);

        // Act
        async Task RemoveStored(int i)
        {
            await using var scope = Services.CreateAsyncScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRequestRepository>();
            await repo.RemoveByIdAsync(storedRequest.Id, user.Id, CancellationToken);
        }
        var result = await WhenAllAsync(RemoveStored, RemoveStored, RemoveStored);

        // Assert
        Assert.Null(result.Exception);
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var count = await dbContext.Requests.CountAsync(
            e => e.UserId == user.Id,
            CancellationToken
        );
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task RemoveById_RemovesNothing_WhenUserRemovesNotHisRequests()
    {
        // Arrange
        var user1 = await CreateUserAsync("user1");
        var storedRequest1 = await CreateRequestForAsync("request1", user1);
        var user2 = await CreateUserAsync("user2");
        var storedRequest2 = await CreateRequestForAsync("request1", user2);

        // Act + Assert
        await using (var scope = Services.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IRequestRepository>();
            await repo.RemoveByIdAsync(storedRequest2.Id, user1.Id, CancellationToken);
            await repo.RemoveByIdAsync(storedRequest1.Id, user2.Id, CancellationToken);
        }

        await using var assertScope = Services.CreateAsyncScope();
        var dbContext = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();
        int count;
        count = await dbContext.Requests.CountAsync(e => e.UserId == user1.Id, CancellationToken);
        Assert.True(count == 1, "Request for user 1 was deleted, but should not have been");
        count = await dbContext.Requests.CountAsync(e => e.UserId == user2.Id, CancellationToken);
        Assert.True(count == 1, "Request for user 2 was deleted, but should not have been");
    }

    [Fact]
    public async Task UpdateExistingAsync_UpdatesRequest()
    {
        // Arrange
        var user = await CreateUserAsync();
        var storedRequest = await CreateRequestForAsync("request", user);

        // Act
        StoredRequest updated;
        await using (var scope = Services.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IRequestRepository>();
            updated = storedRequest with
            {
                Request = new("request 2", SortBy.Name, SortOrder.Desc),
            };
            await repo.UpdateExistingAsync(updated, CancellationToken);
        }

        // Assert
        await using var assertScope = Services.CreateAsyncScope();
        var dbContext = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var all = await dbContext
            .Requests.Where(e => e.UserId == user.Id)
            .Select(e => e.ToStoredRequest())
            .ToListAsync(CancellationToken);
        Assert.Equal([updated], all);
    }

    [Fact]
    public async Task UpdateExistingAsync_ThrowsDomainException_WhenChangingToExistingRequest()
    {
        // Arrange
        var user = await CreateUserAsync();
        var storedRequest1 = await CreateRequestForAsync("request1", user);
        var storedRequest2 = await CreateRequestForAsync("request2", user);

        // Act
        var ex = await Assert.ThrowsAsync<DomainException<ConflictExceptionType>>(async () =>
        {
            await using var scope = Services.CreateAsyncScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRequestRepository>();
            var toUpdate = storedRequest2 with { Request = storedRequest1.Request };
            await repo.UpdateExistingAsync(toUpdate, CancellationToken);
        });
        Assert.Equal(StoredRequestNotUnique, ex.Type);

        // Assert
        await using var assertScope = Services.CreateAsyncScope();
        var dbContext = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var all = await dbContext
            .Requests.Where(e => e.UserId == user.Id)
            .Select(e => e.ToStoredRequest())
            .ToListAsync(CancellationToken);
        Assert.Equal([storedRequest1, storedRequest2], all);
    }

    private static async Task<Task> WhenAllAsync(params Func<int, Task>[] actions)
    {
        Task all = Task.WhenAll([.. actions.Select((act, i) => Task.Run(() => act(i)))]);
        try
        {
            await all;
        }
        catch { }
        return all;
    }

    private async Task<User> CreateUserAsync(
        string name = "username",
        string surname = "usersurname"
    )
    {
        await using var scope = Services.CreateAsyncScope();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        return await userRepo.RegisterNewAsync(name, surname, "P@ssw0rd");
    }

    private async Task<StoredRequest> CreateRequestForAsync(
        string searchString,
        User user,
        SortBy sortBy = SortBy.UnifiedPrice,
        SortOrder sortOrder = SortOrder.Asc
    )
    {
        await using var scope = Services.CreateAsyncScope();
        var repo = scope.ServiceProvider.GetRequiredService<IRequestRepository>();
        return await repo.AddNewAsync(
            new Request(searchString, sortBy, sortOrder),
            user.Id,
            int.MaxValue,
            CancellationToken
        );
    }
}
