using PriceComparer.Application.Users.DTOs;

namespace PriceComparer.Application.Users;

public interface IUserRepository
{
    /// <summary>
    /// Creates user if it's name is unique
    /// </summary>
    /// <exception cref="UserException"/>
    Task<UserId> CreateAsync(UserDto user, CancellationToken cancellationToken);
    Task<UserId?> FindByIdPAsync(IdPInfo idPInfo, CancellationToken cancellationToken);
}
