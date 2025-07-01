using PriceComparer.Domain;

namespace PriceComparer.Web.Repository;

public interface IUserRepository{
    Task<User?> GetUserByNameAsync(string userName);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> SaveAsync(User user);

}