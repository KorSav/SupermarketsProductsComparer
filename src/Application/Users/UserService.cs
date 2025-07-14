using PriceComparer.Domain;

namespace PriceComparer.Application.Users;

public class UserService(IUserRepository userRepo)
{
    private readonly IUserRepository _repo = userRepo;

    public async Task<User> Register(UserDto userDto) =>
        await _repo.FindByIdPAsync(userDto.IdPInfo) ?? await _repo.CreateAsync(userDto);

    public async Task<User> Login(IdPInfo idPInfo) =>
        await _repo.FindByIdPAsync(idPInfo)
        ?? throw new Exception($"The given identity is not registered: {idPInfo}");
}
