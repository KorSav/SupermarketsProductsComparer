using PriceComparer.Domain;

namespace PriceComparer.Application.Users;

public interface IUserRepository
{
    /// <summary>
    /// Creates user if it does not exist, otherwise throws exception
    /// </summary>
    Task<User> CreateAsync(UserDto user);
    Task<User?> FindByIdPAsync(IdPInfo idPInfo);
}
