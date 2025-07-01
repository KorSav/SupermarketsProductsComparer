using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using PriceComparer.Domain;
using PriceComparer.Web.Repository;

namespace PriceComparer.Web.Services;

public class UserService(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<bool> TryRegister(User user)
    {
        User? existingUser =
            await _userRepository.GetUserByNameAsync(user.Name)
            ?? await _userRepository.GetUserByEmailAsync(user.Email);
        if (existingUser is not null)
            return false;
        await _userRepository.SaveAsync(user);
        return true;
    }

    public async Task<bool> IsRegisteredAsync(User user)
    {
        User? foundUser = await _userRepository.GetUserByNameAsync(user.Name);
        if (
            foundUser is not null
            && IsPasswordCorrect(user.Name, user.PasswordHash, foundUser.PasswordHash)
        )
            return true;
        foundUser = await _userRepository.GetUserByEmailAsync(user.Email);
        if (
            foundUser is not null
            && IsPasswordCorrect(user.Name, user.PasswordHash, foundUser.PasswordHash)
        )
            return true;
        return false;
    }

    private bool IsPasswordCorrect(string username, string provided, string hashed)
    {
        PasswordHasher<string> passwordHasher = new();
        var result = passwordHasher.VerifyHashedPassword(username, hashed, provided);
        return result != PasswordVerificationResult.Failed;
    }

    public async Task<User> RefillAsync(User user)
    {
        if (string.IsNullOrEmpty(user.Name) is false)
        {
            return await _userRepository.GetUserByNameAsync(user.Name)
                ?? throw new AuthenticationException(
                    $"User with name '{user.Name}' is not found in db"
                );
        }
        else if (string.IsNullOrEmpty(user.Email) is false)
        {
            return await _userRepository.GetUserByEmailAsync(user.Email)
                ?? throw new AuthenticationException(
                    $"User with email '{user.Email}' is not found in db"
                );
        }
        throw new ArgumentException("Unable to refill user with null properties");
    }
}
