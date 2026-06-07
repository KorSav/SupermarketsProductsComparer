using ApplicationCore.Entities;
using ApplicationCore.Services;
using NSubstitute;

namespace ApplicationCore.UnitTests.Services;

public sealed class UserServiceTests
{
    [Fact]
    public async Task TryRegisterAsync_ReturnsRegisteredUserWhenUserDoesNotExist()
    {
        var registeredUser = new User(Guid.NewGuid(), "John", "john@example.com");
        var repo = Substitute.For<IUserRepository>();
        repo.FindByNameAndEmailAsync("John", "john@example.com")
            .Returns(Task.FromResult<User?>(null));
        repo.RegisterNewAsync("John", "john@example.com", "password")
            .Returns(Task.FromResult(registeredUser));
        var service = new UserService(repo);

        var result = await service.TryRegisterAsync("John", "john@example.com", "password");

        Assert.True(result.IsSuccess);
        Assert.Equal(registeredUser, result.Value);
        Assert.Empty(result.ErrorList);
        await repo.Received(1).RegisterNewAsync("John", "john@example.com", "password");
    }

    [Fact]
    public async Task TryRegisterAsync_ReturnsFailureWhenUserAlreadyExists()
    {
        var existingUser = new User(Guid.NewGuid(), "John", "john@example.com");
        var repo = Substitute.For<IUserRepository>();
        repo.FindByNameAndEmailAsync("John", "john@example.com")
            .Returns(Task.FromResult<User?>(existingUser));
        var service = new UserService(repo);

        var result = await service.TryRegisterAsync("John", "john@example.com", "password");

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal(2, result.ErrorList.Count);
        Assert.Contains(result.ErrorList, e => e.Reason == nameof(User.Name));
        Assert.Contains(result.ErrorList, e => e.Reason == nameof(User.Email));
        await repo.DidNotReceiveWithAnyArgs().RegisterNewAsync(default!, default!, default!);
    }

    [Fact]
    public async Task TryLoginAsync_ReturnsUserWhenCredentialsAreValid()
    {
        var user = new User(Guid.NewGuid(), "John", "john@example.com");
        var repo = Substitute.For<IUserRepository>();
        repo.FindByNameAndEmailAsync("John", "john@example.com")
            .Returns(Task.FromResult<User?>(user));
        repo.IsPasswordValidAsync(user.Id, "password").Returns(Task.FromResult(true));
        var service = new UserService(repo);

        var result = await service.TryLoginAsync("John", "john@example.com", "password");

        Assert.True(result.IsSuccess);
        Assert.Equal(user, result.Value);
        Assert.Empty(result.ErrorList);
        await repo.Received(1).IsPasswordValidAsync(user.Id, "password");
    }

    [Fact]
    public async Task TryLoginAsync_ReturnsNameAndEmailErrorsWhenUserDoesNotExist()
    {
        var repo = Substitute.For<IUserRepository>();
        repo.FindByNameAndEmailAsync("John", "john@example.com")
            .Returns(Task.FromResult<User?>(null));
        var service = new UserService(repo);

        var result = await service.TryLoginAsync("John", "john@example.com", "password");

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal(2, result.ErrorList.Count);
        Assert.Contains(result.ErrorList, e => e.Reason == nameof(User.Name));
        Assert.Contains(result.ErrorList, e => e.Reason == nameof(User.Email));
        await repo.DidNotReceiveWithAnyArgs().IsPasswordValidAsync(default, default!);
    }

    [Fact]
    public async Task TryLoginAsync_ReturnsPasswordErrorWhenPasswordIsInvalid()
    {
        var user = new User(Guid.NewGuid(), "John", "john@example.com");
        var repo = Substitute.For<IUserRepository>();
        repo.FindByNameAndEmailAsync("John", "john@example.com")
            .Returns(Task.FromResult<User?>(user));
        repo.IsPasswordValidAsync(user.Id, "wrong-password").Returns(Task.FromResult(false));
        var service = new UserService(repo);

        var result = await service.TryLoginAsync("John", "john@example.com", "wrong-password");

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorList);
        Assert.Equal("password", result.ErrorList[0].Reason);
        await repo.Received(1).IsPasswordValidAsync(user.Id, "wrong-password");
    }
}
