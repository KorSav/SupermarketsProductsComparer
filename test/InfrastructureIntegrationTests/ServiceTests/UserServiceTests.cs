using ApplicationCore.Entities;
using ApplicationCore.Services;
using ApplicationCore.Utils;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfrastructureIntegrationTests.ServiceTests;

[Collection(Collections.Container1)]
public class UserServiceTests(DbContainerFixture _) : DbPerTestCaseBase(_)
{
    [Fact]
    public async Task TryRegister_CreatesNewUser()
    {
        // Arrange + Act
        User registered;
        await using (var scope = Services.CreateAsyncScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<UserService>();
            var result = await service.TryRegisterAsync("user name", "user surname", "P@ssw0rd");
            registered = result.Value!;
        }

        // Assert
        await using var assertScope = Services.CreateAsyncScope();
        var dbContext = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var act = await dbContext.Users.ToListAsync(CancellationToken);
        Assert.Equal([registered], act.Select(e => e.ToUser()));
        var efUser = act[0];
        Assert.DoesNotContain("P@ssw0rd", efUser.PasswordHash);
    }

    [Fact]
    public async Task TryRegister_ReturnsError_WhenUserExists()
    {
        // Arrange
        User registered;
        await using (var scope = Services.CreateAsyncScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<UserService>();
            var result = await service.TryRegisterAsync("user name", "user surname", "P@ssw0rd");
            registered = result.Value!;
        }

        // Act
        Result<User> loginResult;
        await using (var scope = Services.CreateAsyncScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<UserService>();
            loginResult = await service.TryRegisterAsync("user name", "user surname", "P@ssw0rd");
        }

        // Assert
        Assert.False(loginResult.IsSuccess);
        var actReasons = loginResult.ErrorList.Select(e => e.Reason).ToList();
        Assert.Equivalent(new[] { "Name", "Surname" }, actReasons, strict: true);
    }

    [Fact]
    public async Task TryLogin_Succeeds_WhenUserRegistered()
    {
        // Arrange
        User registered;
        await using (var scope = Services.CreateAsyncScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<UserService>();
            var result = await service.TryRegisterAsync("user name", "user surname", "P@ssw0rd");
            registered = result.Value!;
        }

        // Act
        Result<User> loginResult;
        await using (var scope = Services.CreateAsyncScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<UserService>();
            loginResult = await service.TryLoginAsync("user name", "user surname", "P@ssw0rd");
        }

        // Assert
        Assert.Equal(registered, loginResult.Value);
        Assert.Equal([], loginResult.ErrorList);
    }
}
