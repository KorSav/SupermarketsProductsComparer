using ApplicationCore.Entities;
using ApplicationCore.Utils;

namespace ApplicationCore.Services;

public class UserService(IUserRepository userRepo)
{
    public async Task<Result<User>> TryRegisterAsync(string name, string surname, string password)
    {
        User? existingUser = await userRepo.FindByNameAndSurnameAsync(name, surname);
        if (existingUser is not null)
            return Result.Failure(
                new TypedError<User>(
                    nameof(User.Name),
                    "User with given name and surname pair already exists"
                ),
                new TypedError<User>(
                    nameof(User.Email),
                    "User with given name and surname pair already exists"
                )
            );
        return await userRepo.RegisterNewAsync(name, surname, password);
    }

    public async Task<Result<User>> TryLoginAsync(string name, string surname, string password)
    {
        User? existing = await userRepo.FindByNameAndSurnameAsync(name, surname);
        if (existing is null)
            return Result.Failure(
                new TypedError<User>(
                    nameof(User.Name),
                    "User with given name and surname pair is not registered"
                ),
                new TypedError<User>(
                    nameof(User.Email),
                    "User with given name and surname pair is not registered"
                )
            );
        if (await userRepo.IsPasswordValidAsync(existing.Id, password))
            return existing;

        return Result.Failure<User>(
            new Error(nameof(password), "User with given name and surname pair is not registered")
        );
    }
}
