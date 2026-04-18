using program.Domain;

namespace program.Repository;

public interface IUserRepository{
    Task<User?> GetUserByNameAsync(string userName);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> SaveAsync(User user);

}