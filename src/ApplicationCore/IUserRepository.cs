using ApplicationCore.Entities;

namespace ApplicationCore;

public interface IUserRepository
{
    Task<User?> FindByNameAndEmailAsync(string name, string email);
    Task<User> RegisterNewAsync(string name, string email, string password);
    Task<bool> IsPasswordValidAsync(Guid userId, string password);
}
