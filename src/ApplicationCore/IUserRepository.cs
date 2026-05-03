using ApplicationCore.Entities;

namespace ApplicationCore;

public interface IUserRepository
{
    Task<User?> FindByNameAndSurnameAsync(string name, string surname);
    Task<User> RegisterNewAsync(string name, string surname, string password);
    Task<bool> IsPasswordValidAsync(Guid userId, string password);
}
