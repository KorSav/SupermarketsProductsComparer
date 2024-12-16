using Microsoft.AspNetCore.Identity;
using program.Domain;
using program.Repository;

namespace program.Services;

public class UserService(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<bool> TryRegister(User user)
    {
        User? existingUser = await _userRepository.GetUserByNameAsync(user.Name)
            ?? await _userRepository.GetUserByEmailAsync(user.Email);
        if (existingUser is not null)
            return false;
        await _userRepository.SaveAsync(user);
        return true;
    }

    public async Task<bool> IsRegisteredAsync(User user)
    {
        User? foundUser = await _userRepository.GetUserByNameAsync(user.Name);
        if (foundUser is not null &&
            IsPasswordCorrect(user.Name, user.PasswordHash, foundUser.PasswordHash))
            return true;
        foundUser = await _userRepository.GetUserByEmailAsync(user.Email);
        if (foundUser is not null &&
            IsPasswordCorrect(user.Name, user.PasswordHash, foundUser.PasswordHash))
            return true;
        return false;
    }

    private bool IsPasswordCorrect(string username, string provided, string hashed)
    {
        PasswordHasher<string> passwordHasher = new();
        var result = passwordHasher.VerifyHashedPassword(username, hashed, provided);
        return result != PasswordVerificationResult.Failed;
    }
}