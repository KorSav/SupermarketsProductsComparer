using PriceComparer.Application.Users.DTOs;

namespace PriceComparer.Application.Users;

public class UserService(IUserRepository userRepo)
{
    private readonly IUserRepository _repo = userRepo;

    public async Task<UserId> Register(UserDto userDto, CancellationToken cancellationToken) =>
        await _repo.FindByIdPAsync(userDto.IdPInfo, cancellationToken)
        ?? await _repo.CreateAsync(userDto, cancellationToken);

    public async Task<UserId> Login(IdPInfo idPInfo, CancellationToken cancellationToken) =>
        await _repo.FindByIdPAsync(idPInfo, cancellationToken)
        ?? throw new UserException(UserException.Code.NonregisteredLogin);
}
