using System.Security.Claims;
using ApplicationCore.Entities;

namespace WebApp.Mappings;

public static class ClaimsPrincipalExtenstions
{
    public static User ToUser(this ClaimsPrincipal claimsPrincipal)
    {
        var idClaim = claimsPrincipal.FindFirst(c => c.Type is ClaimTypes.NameIdentifier);
        _ = Guid.TryParse(idClaim?.Value, out var id);
        var nameClaim = claimsPrincipal.FindFirst(c => c.Type is ClaimTypes.Name);
        var emailClaim = claimsPrincipal.FindFirst(c => c.Type is ClaimTypes.Email);
        if (id == default || nameClaim is null || emailClaim is null)
            throw new InvalidOperationException("User has no neither 'name' nor 'surname' claims");
        return new User(id, nameClaim.Value, emailClaim.Value);
    }

    public static ClaimsPrincipal ToClaimsPrincipal(this User user, string authnScheme)
    {
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
        };
        var claimsIdentity = new ClaimsIdentity(claims, authnScheme);
        return new ClaimsPrincipal(claimsIdentity);
    }
}
