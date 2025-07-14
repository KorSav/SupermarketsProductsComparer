namespace PriceComparer.Application.Users;

public enum IdP : byte
{
    LocalLogin,
    Google,
}

public record IdPInfo(IdP Provider, string Login);
