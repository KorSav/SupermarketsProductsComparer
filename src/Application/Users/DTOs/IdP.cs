namespace PriceComparer.Application.Users.DTOs;

public enum IdP : byte
{
    LocalLogin,
    Google,
}

public record IdPInfo(IdP Provider, string UserId);
